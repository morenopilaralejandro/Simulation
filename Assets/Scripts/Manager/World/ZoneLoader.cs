using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Handles the actual loading/unloading of scenes via Addressables.
/// </summary>
public class ZoneLoader : MonoBehaviour
{
    public static ZoneLoader Instance { get; private set; }

    // Track loaded scene handles so we can unload them
    private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes
        = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

    // Track in-flight loads — maps to the pending Task so duplicate callers
    // can await the same operation instead of silently returning false
    private readonly Dictionary<string, Task<bool>> _loadingScenes
        = new Dictionary<string, Task<bool>>();

    // Track in-flight unloads to prevent double-unload races
    private readonly Dictionary<string, Task<bool>> _unloadingScenes
        = new Dictionary<string, Task<bool>>();

    // Reusable list to avoid allocating in UnloadAllScenesAsync every call
    private readonly List<string> _tempAddressList = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Load a scene additively via Addressables.
    /// Returns true if the scene was newly loaded, false if already loaded.
    /// Duplicate calls while loading await the same operation.
    /// </summary>
    public Task<bool> LoadSceneAsync(string sceneAddress)
    {
        if (_loadedScenes.ContainsKey(sceneAddress))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogManager.Trace(string.Concat("[ZoneLoader] Scene already loaded: ", sceneAddress));
#endif
            return Task.FromResult(false);
        }

        Task<bool> pendingTask;
        if (_loadingScenes.TryGetValue(sceneAddress, out pendingTask))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogManager.Trace(string.Concat("[ZoneLoader] Scene already loading: ", sceneAddress));
#endif
            return pendingTask;
        }

        Task<bool> task = LoadSceneInternalAsync(sceneAddress);
        _loadingScenes[sceneAddress] = task;
        return task;
    }

    private async Task<bool> LoadSceneInternalAsync(string sceneAddress)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogManager.Trace(string.Concat("[ZoneLoader] Loading scene: ", sceneAddress));
#endif

        try
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(
                sceneAddress,
                LoadSceneMode.Additive,
                true // activateOnLoad
            );

            await handle.Task;
            await SceneLoader.Instance.AwaitSceneObjectLoaders(sceneAddress);

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedScenes[sceneAddress] = handle;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                LogManager.Trace(string.Concat("[ZoneLoader] Successfully loaded: ", sceneAddress,
                    " | Handle valid: ", handle.IsValid().ToString(),
                    " | Scene: ", handle.Result.Scene.name,
                    " | Scene isLoaded: ", handle.Result.Scene.isLoaded.ToString()));
#endif
                return true;
            }

            LogManager.Error(string.Concat("[ZoneLoader] Failed to load scene: ", sceneAddress,
                " | Status: ", handle.Status.ToString()));

            // Release the failed handle to avoid leaking resources
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            return false;
        }
        catch (System.Exception e)
        {
            LogManager.Error(string.Concat("[ZoneLoader] Exception loading ", sceneAddress, ": ", e.Message));
            return false;
        }
        finally
        {
            _loadingScenes.Remove(sceneAddress);
        }
    }

    /// <summary>
    /// Unload a previously loaded scene.
    /// If the scene is currently being loaded, waits for the load to finish first.
    /// If the scene is currently being unloaded, awaits the existing unload.
    /// </summary>
    public Task<bool> UnloadSceneAsync(string sceneAddress)
    {
        // If already unloading, return the existing task to prevent double-unload
        Task<bool> pendingUnload;
        if (_unloadingScenes.TryGetValue(sceneAddress, out pendingUnload))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogManager.Trace(string.Concat("[ZoneLoader] Scene already unloading: ", sceneAddress));
#endif
            return pendingUnload;
        }

        Task<bool> task = UnloadSceneInternalAsync(sceneAddress);
        _unloadingScenes[sceneAddress] = task;
        return task;
    }

    private async Task<bool> UnloadSceneInternalAsync(string sceneAddress)
    {
        try
        {
            // If the scene is still loading, wait for the load to complete first
            Task<bool> pendingLoad;
            if (_loadingScenes.TryGetValue(sceneAddress, out pendingLoad))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                LogManager.Warning(string.Concat("[ZoneLoader] Waiting for in-flight load before unloading: ", sceneAddress));
#endif
                await pendingLoad;
            }

            AsyncOperationHandle<SceneInstance> handle;
            if (!_loadedScenes.TryGetValue(sceneAddress, out handle))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                LogManager.Warning(string.Concat("[ZoneLoader] Cannot unload — not tracked: ", sceneAddress));
#endif
                return await TryFallbackUnload(sceneAddress);
            }

            // Remove immediately to prevent other concurrent calls from grabbing the same handle
            _loadedScenes.Remove(sceneAddress);

            // Validate the handle before attempting to use it
            if (!handle.IsValid())
            {
                LogManager.Warning(string.Concat("[ZoneLoader] Handle invalid (IsValid=false), attempting fallback unload: ", sceneAddress));
                return await TryFallbackUnload(sceneAddress);
            }

            // Grab the scene reference BEFORE we attempt unload so fallback can use it
            Scene targetScene = default;
            string targetSceneName = sceneAddress;
            try
            {
                targetScene = handle.Result.Scene;
                targetSceneName = targetScene.name;
            }
            catch (System.Exception e)
            {
                LogManager.Warning(string.Concat("[ZoneLoader] Could not read scene from handle: ", e.Message));
            }

            // Check if scene is already gone
            if (!targetScene.IsValid() || !targetScene.isLoaded)
            {
                LogManager.Warning(string.Concat("[ZoneLoader] Handle valid but scene already unloaded: ", sceneAddress));
                try
                {
                    Addressables.Release(handle);
                }
                catch (System.Exception releaseEx)
                {
                    LogManager.Warning(string.Concat("[ZoneLoader] Failed to release stale handle: ", releaseEx.Message));
                }
                return true;
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogManager.Trace(string.Concat("[ZoneLoader] Unloading scene: ", sceneAddress,
                " | Handle valid: ", handle.IsValid().ToString(),
                " | Scene: ", targetSceneName,
                " | Scene isLoaded: ", targetScene.isLoaded.ToString()));
#endif

            // ==========================================================
            // PRIMARY UNLOAD: Try Addressables.UnloadSceneAsync
            // ==========================================================
            bool addressablesSucceeded = false;
            try
            {
                AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(handle, true);
                await unloadHandle.Task;

                if (unloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    LogManager.Trace(string.Concat("[ZoneLoader] Successfully unloaded via Addressables: ", sceneAddress));
#endif
                    addressablesSucceeded = true;
                }
                else
                {
                    LogManager.Error(string.Concat("[ZoneLoader] Addressables unload failed: ", sceneAddress,
                        " | Status: ", unloadHandle.Status.ToString()));
                }
            }
            catch (System.Exception e)
            {
                LogManager.Warning(string.Concat("[ZoneLoader] Addressables.UnloadSceneAsync threw: ", e.Message,
                    " — will attempt SceneManager fallback"));
            }

            if (addressablesSucceeded)
            {
                return true;
            }

            // ==========================================================
            // FALLBACK: Try SceneManager.UnloadSceneAsync directly
            // ==========================================================
            // The Addressables call failed/threw, but the scene might
            // still be loaded (or might have been torn down by the
            // failed call). Check and clean up.
            return await TryFallbackUnloadByScene(targetScene, targetSceneName, sceneAddress);
        }
        catch (System.Exception e)
        {
            LogManager.Error(string.Concat("[ZoneLoader] Outer exception unloading ", sceneAddress, ": ", e.Message));
            return await TryFallbackUnload(sceneAddress);
        }
        finally
        {
            _unloadingScenes.Remove(sceneAddress);
        }
    }

    /// <summary>
    /// Fallback using a Scene reference we already have.
    /// </summary>
    private async Task<bool> TryFallbackUnloadByScene(Scene scene, string sceneName, string sceneAddress)
    {
        // Check if the scene is still actually loaded
        if (scene.IsValid() && scene.isLoaded)
        {
            LogManager.Warning(string.Concat("[ZoneLoader] Fallback: unloading via SceneManager: ", sceneName));
            try
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene);
                if (unloadOp != null)
                {
                    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                    unloadOp.completed += (op) => tcs.SetResult(true);
                    await tcs.Task;

                    LogManager.Trace(string.Concat("[ZoneLoader] Fallback: successfully unloaded via SceneManager: ", sceneName));
                    return true;
                }
                else
                {
                    LogManager.Error(string.Concat("[ZoneLoader] Fallback: SceneManager.UnloadSceneAsync returned null for: ", sceneName));
                }
            }
            catch (System.Exception e)
            {
                LogManager.Error(string.Concat("[ZoneLoader] Fallback: exception during SceneManager unload: ", e.Message));
            }
        }

        // Scene is already gone — the Addressables call may have partially succeeded
        // before throwing. Check all loaded scenes to be sure.
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == sceneName || loadedScene.name == sceneAddress)
            {
                if (loadedScene.isLoaded)
                {
                    LogManager.Warning(string.Concat("[ZoneLoader] Fallback: found scene by name scan, unloading: ", loadedScene.name));
                    try
                    {
                        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(loadedScene);
                        if (unloadOp != null)
                        {
                            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                            unloadOp.completed += (op) => tcs.SetResult(true);
                            await tcs.Task;
                            return true;
                        }
                    }
                    catch (System.Exception e)
                    {
                        LogManager.Error(string.Concat("[ZoneLoader] Fallback: scan unload exception: ", e.Message));
                    }
                }
            }
        }

        LogManager.Warning(string.Concat("[ZoneLoader] Fallback: scene appears already unloaded: ", sceneAddress));
        return true;
    }

    /// <summary>
    /// Fallback: attempt to unload the scene by searching SceneManager.
    /// Used when we don't have a Scene reference.
    /// </summary>
    private async Task<bool> TryFallbackUnload(string sceneAddress)
    {
        string sceneName = ExtractSceneNameFromAddress(sceneAddress);

        // Search all loaded scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == sceneName || loadedScene.name == sceneAddress ||
                loadedScene.path.Contains(sceneName))
            {
                if (loadedScene.isLoaded)
                {
                    LogManager.Warning(string.Concat("[ZoneLoader] Fallback: unloading via SceneManager: ", loadedScene.name));
                    try
                    {
                        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(loadedScene);
                        if (unloadOp != null)
                        {
                            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                            unloadOp.completed += (op) => tcs.SetResult(true);
                            await tcs.Task;

                            LogManager.Trace(string.Concat("[ZoneLoader] Fallback: successfully unloaded via SceneManager: ", loadedScene.name));
                            return true;
                        }
                        else
                        {
                            LogManager.Error(string.Concat("[ZoneLoader] Fallback: SceneManager.UnloadSceneAsync returned null for: ", loadedScene.name));
                        }
                    }
                    catch (System.Exception e)
                    {
                        LogManager.Error(string.Concat("[ZoneLoader] Fallback: exception during SceneManager unload: ", e.Message));
                        return false;
                    }
                }
            }
        }

        LogManager.Warning(string.Concat("[ZoneLoader] Fallback: scene not found in SceneManager, assuming already unloaded: ", sceneAddress));
        return true;
    }

    /// <summary>
    /// Extracts a likely scene name from an Addressable address.
    /// </summary>
    private string ExtractSceneNameFromAddress(string sceneAddress)
    {
        int lastSlash = sceneAddress.LastIndexOf('/');
        if (lastSlash >= 0 && lastSlash < sceneAddress.Length - 1)
        {
            sceneAddress = sceneAddress.Substring(lastSlash + 1);
        }

        if (sceneAddress.EndsWith(".unity"))
        {
            sceneAddress = sceneAddress.Substring(0, sceneAddress.Length - 6);
        }

        return sceneAddress;
    }

    /// <summary>
    /// Unload all currently loaded scenes sequentially to limit memory spikes.
    /// </summary>
    public async Task UnloadAllScenesAsync()
    {
        _tempAddressList.Clear();
        Dictionary<string, AsyncOperationHandle<SceneInstance>>.KeyCollection keys = _loadedScenes.Keys;
        foreach (string key in keys)
        {
            _tempAddressList.Add(key);
        }

        for (int i = 0, count = _tempAddressList.Count; i < count; i++)
        {
            await UnloadSceneAsync(_tempAddressList[i]);
        }

        _tempAddressList.Clear();
    }

    public bool IsSceneLoaded(string sceneAddress)
    {
        return _loadedScenes.ContainsKey(sceneAddress);
    }

    public bool IsSceneLoading(string sceneAddress)
    {
        return _loadingScenes.ContainsKey(sceneAddress);
    }

    public bool IsSceneUnloading(string sceneAddress)
    {
        return _unloadingScenes.ContainsKey(sceneAddress);
    }

    [ContextMenu("Debug: Log Tracked Scenes")]
    public void DebugLogTrackedScenes()
    {
        LogManager.Trace("[ZoneLoader] === Tracked Scenes ===");
        LogManager.Trace(string.Concat("[ZoneLoader] Loaded: ", _loadedScenes.Count.ToString()));
        foreach (var kvp in _loadedScenes)
        {
            bool handleValid = kvp.Value.IsValid();
            string sceneInfo = "N/A";
            if (handleValid && kvp.Value.Status == AsyncOperationStatus.Succeeded)
            {
                Scene s = kvp.Value.Result.Scene;
                sceneInfo = string.Concat("Scene.IsValid=", s.IsValid().ToString(),
                    " isLoaded=", (s.IsValid() ? s.isLoaded.ToString() : "N/A"));
            }
            LogManager.Trace(string.Concat("  ", kvp.Key,
                " | HandleValid=", handleValid.ToString(),
                " | Status=", kvp.Value.Status.ToString(),
                " | ", sceneInfo));
        }
        LogManager.Trace(string.Concat("[ZoneLoader] Loading: ", _loadingScenes.Count.ToString()));
        LogManager.Trace(string.Concat("[ZoneLoader] Unloading: ", _unloadingScenes.Count.ToString()));
        LogManager.Trace(string.Concat("[ZoneLoader] Unity SceneManager scenes: ", SceneManager.sceneCount.ToString()));
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            LogManager.Trace(string.Concat("  ", s.name, " | isLoaded=", s.isLoaded.ToString(), " | path=", s.path));
        }
    }
}
