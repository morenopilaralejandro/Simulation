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
    private Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes
        = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

    // Track in-flight loads to avoid duplicate loading
    private HashSet<string> _loadingScenes = new HashSet<string>();

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
    /// </summary>
    public async Task<bool> LoadSceneAsync(string sceneAddress)
    {
        if (_loadedScenes.ContainsKey(sceneAddress))
        {
            LogManager.Trace($"[ZoneLoader] Scene already loaded: {sceneAddress}");
            return false;
        }

        if (_loadingScenes.Contains(sceneAddress))
        {
            LogManager.Trace($"[ZoneLoader] Scene already loading: {sceneAddress}");
            return false;
        }

        _loadingScenes.Add(sceneAddress);

        LogManager.Trace($"[ZoneLoader] Loading scene: {sceneAddress}");

        try
        {
            var handle = Addressables.LoadSceneAsync(
                sceneAddress,
                LoadSceneMode.Additive,
                activateOnLoad: true
            );

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedScenes[sceneAddress] = handle;
                _loadingScenes.Remove(sceneAddress);
                LogManager.Trace($"[ZoneLoader] Successfully loaded: {sceneAddress}");
                return true;
            }
            else
            {
                LogManager.Error($"[ZoneLoader] Failed to load scene: {sceneAddress}");
                _loadingScenes.Remove(sceneAddress);
                return false;
            }
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[ZoneLoader] Exception loading {sceneAddress}: {e.Message}");
            _loadingScenes.Remove(sceneAddress);
            return false;
        }
    }

    /// <summary>
    /// Unload a previously loaded scene.
    /// </summary>
    public async Task<bool> UnloadSceneAsync(string sceneAddress)
    {
        if (!_loadedScenes.TryGetValue(sceneAddress, out var handle))
        {
            LogManager.Warning($"[ZoneLoader] Cannot unload — not loaded: {sceneAddress}");
            return false;
        }

        LogManager.Trace($"[ZoneLoader] Unloading scene: {sceneAddress}");

        try
        {
            var unloadHandle = Addressables.UnloadSceneAsync(handle, true);
            await unloadHandle.Task;

            _loadedScenes.Remove(sceneAddress);

            if (unloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                LogManager.Trace($"[ZoneLoader] Successfully unloaded: {sceneAddress}");
                return true;
            }
            else
            {
                LogManager.Error($"[ZoneLoader] Failed to unload: {sceneAddress}");
                return false;
            }
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[ZoneLoader] Exception unloading {sceneAddress}: {e.Message}");
            _loadedScenes.Remove(sceneAddress);
            return false;
        }
    }

    /// <summary>
    /// Unload all currently loaded scenes.
    /// </summary>
    public async Task UnloadAllScenesAsync()
    {
        var addresses = new List<string>(_loadedScenes.Keys);

        var tasks = new List<Task>();
        foreach (var address in addresses)
        {
            tasks.Add(UnloadSceneAsync(address));
        }

        await Task.WhenAll(tasks);
    }

    public bool IsSceneLoaded(string sceneAddress)
    {
        return _loadedScenes.ContainsKey(sceneAddress);
    }

    public bool IsSceneLoading(string sceneAddress)
    {
        return _loadingScenes.Contains(sceneAddress);
    }
}
