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

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedScenes[sceneAddress] = handle;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                LogManager.Trace(string.Concat("[ZoneLoader] Successfully loaded: ", sceneAddress));
#endif
                return true;
            }

            LogManager.Error(string.Concat("[ZoneLoader] Failed to load scene: ", sceneAddress));
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
    /// </summary>
    public async Task<bool> UnloadSceneAsync(string sceneAddress)
    {
        AsyncOperationHandle<SceneInstance> handle;
        if (!_loadedScenes.TryGetValue(sceneAddress, out handle))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            LogManager.Warning(string.Concat("[ZoneLoader] Cannot unload — not loaded: ", sceneAddress));
#endif
            return false;
        }

        // Remove immediately to prevent double-unload from concurrent calls
        _loadedScenes.Remove(sceneAddress);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogManager.Trace(string.Concat("[ZoneLoader] Unloading scene: ", sceneAddress));
#endif

        try
        {
            AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(handle, true);
            await unloadHandle.Task;

            if (unloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                LogManager.Trace(string.Concat("[ZoneLoader] Successfully unloaded: ", sceneAddress));
#endif
                return true;
            }

            LogManager.Error(string.Concat("[ZoneLoader] Failed to unload: ", sceneAddress));
            return false;
        }
        catch (System.Exception e)
        {
            LogManager.Error(string.Concat("[ZoneLoader] Exception unloading ", sceneAddress, ": ", e.Message));
            return false;
        }
    }

    /// <summary>
    /// Unload all currently loaded scenes sequentially to limit memory spikes.
    /// </summary>
    public async Task UnloadAllScenesAsync()
    {
        // Copy keys into reusable list to avoid modifying dict while iterating
        _tempAddressList.Clear();
        Dictionary<string, AsyncOperationHandle<SceneInstance>>.KeyCollection keys = _loadedScenes.Keys;
        foreach (string key in keys)
        {
            _tempAddressList.Add(key);
        }

        // Sequential unloading reduces peak memory pressure on low-end devices
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
}
