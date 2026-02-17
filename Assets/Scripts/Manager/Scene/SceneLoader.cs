using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private SceneGroupRegistry registry;
    [SerializeField] private SceneData loadingScreenScene;

    private HashSet<string> loadedScenes = new HashSet<string>();
    private SceneGroup currentGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        registry = SceneGroupRegistry.Instance;
    }

    // ──────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────

    /// <summary>
    /// Loads a single scene by its SceneData asset.
    /// </summary>
    public void LoadScene(SceneData sceneData, Action onComplete = null)
    {
        if (sceneData == null)
        {
            Debug.LogError("SceneLoader: Attempted to load a null SceneData.");
            return;
        }

        if (!IsValidForCurrentBuild(sceneData))
        {
            Debug.LogWarning($"SceneLoader: Scene '{sceneData.sceneName}' is debug-only and excluded from this build.");
            onComplete?.Invoke();
            return;
        }

        if (IsSceneLoaded(sceneData.sceneName))
        {
            Debug.LogWarning($"SceneLoader: Scene '{sceneData.sceneName}' is already loaded.");
            onComplete?.Invoke();
            return;
        }

        if (sceneData.useLoadingScreen)
        {
            StartCoroutine(LoadWithLoadingScreen(new List<SceneData> { sceneData }, null, onComplete));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(sceneData.sceneName, onComplete));
        }
    }

    /// <summary>
    /// Unloads a single scene by its SceneData asset.
    /// </summary>
    public void UnloadScene(SceneData sceneData, Action onComplete = null)
    {
        if (sceneData == null)
        {
            Debug.LogError("SceneLoader: Attempted to unload a null SceneData.");
            return;
        }

        if (!IsSceneLoaded(sceneData.sceneName))
        {
            Debug.LogWarning($"SceneLoader: Scene '{sceneData.sceneName}' is not loaded.");
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(UnloadSceneAsync(sceneData.sceneName, onComplete));
    }

    /// <summary>
    /// Loads all valid scenes in a SceneGroup. 
    /// Remembers what was loaded and automatically unloads it before loading the new group.
    /// </summary>
    public void LoadGroup(SceneGroup group, Action onComplete = null)
    {
        if (group == null)
        {
            Debug.LogError("SceneLoader: Attempted to load a null SceneGroup.");
            return;
        }

        List<SceneData> validScenes = group.GetValidScenes();

        if (validScenes.Count == 0)
        {
            Debug.LogWarning($"SceneLoader: Group '{group.groupName}' has no valid scenes.");
            onComplete?.Invoke();
            return;
        }

        SceneGroup previousGroup = currentGroup;
        currentGroup = group;

        List<SceneData> scenesToUnload = null;
        if (previousGroup != null)
        {
            scenesToUnload = previousGroup.GetValidScenes();
        }

        if (group.useLoadingScreen || (previousGroup != null && previousGroup.useLoadingScreen))
        {
            StartCoroutine(LoadWithLoadingScreen(validScenes, scenesToUnload, onComplete));
        }
        else
        {
            StartCoroutine(TransitionSequentially(scenesToUnload, validScenes, onComplete));
        }
    }

    /// <summary>
    /// Unloads all valid scenes in a SceneGroup.
    /// </summary>
    public void UnloadGroup(SceneGroup group, Action onComplete = null)
    {
        if (group == null)
        {
            Debug.LogError("SceneLoader: Attempted to unload a null SceneGroup.");
            return;
        }

        List<SceneData> validScenes = group.GetValidScenes();
        StartCoroutine(UnloadScenesSequentially(validScenes, onComplete));
    }

    /// <summary>
    /// Loads a scene group by name, looked up from the registry.
    /// </summary>
    public void LoadGroupByName(string groupName, Action onComplete = null)
    {
        SceneGroup group = registry.GetGroupByName(groupName);
        if (group != null)
        {
            LoadGroup(group, onComplete);
        }
    }

    /// <summary>
    /// Unloads a scene group by name, looked up from the registry.
    /// </summary>
    public void UnloadGroupByName(string groupName, Action onComplete = null)
    {
        SceneGroup group = registry.GetGroupByName(groupName);
        if (group != null)
        {
            UnloadGroup(group, onComplete);
        }
    }

    /// <summary>
    /// Transitions from one group to another, unloading the old group 
    /// and loading the new one, optionally using a loading screen.
    /// </summary>
    public void TransitionGroups(SceneGroup fromGroup, SceneGroup toGroup, Action onComplete = null)
    {
        if (fromGroup == null || toGroup == null)
        {
            Debug.LogError("SceneLoader: TransitionGroups requires both a 'from' and 'to' group.");
            return;
        }

        List<SceneData> scenesToUnload = fromGroup.GetValidScenes();
        List<SceneData> scenesToLoad = toGroup.GetValidScenes();

        bool useLoadingScreen = fromGroup.useLoadingScreen || toGroup.useLoadingScreen;

        if (useLoadingScreen)
        {
            StartCoroutine(LoadWithLoadingScreen(scenesToLoad, scenesToUnload, onComplete));
        }
        else
        {
            StartCoroutine(TransitionSequentially(scenesToUnload, scenesToLoad, onComplete));
        }
    }

    // ──────────────────────────────────────────────
    // Coroutines
    // ──────────────────────────────────────────────

    private IEnumerator LoadSceneAsync(string sceneName, Action onComplete = null)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (operation == null)
        {
            Debug.LogError($"SceneLoader: Failed to start loading scene '{sceneName}'.");
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        yield return AwaitTask(AwaitSceneObjectLoaders(sceneName));

        loadedScenes.Add(sceneName);
        onComplete?.Invoke();
    }

    private IEnumerator UnloadSceneAsync(string sceneName, Action onComplete = null)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError($"SceneLoader: Failed to start unloading scene '{sceneName}'.");
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        loadedScenes.Remove(sceneName);
        onComplete?.Invoke();
    }

    private IEnumerator LoadScenesSequentially(List<SceneData> scenes, Action onComplete = null)
    {
        foreach (SceneData scene in scenes)
        {
            if (scene == null) continue;
            if (IsSceneLoaded(scene.sceneName)) continue;

            yield return LoadSceneAsync(scene.sceneName);
        }

        onComplete?.Invoke();
    }

    private IEnumerator UnloadScenesSequentially(List<SceneData> scenes, Action onComplete = null)
    {
        foreach (SceneData scene in scenes)
        {
            if (scene == null) continue;
            if (!IsSceneLoaded(scene.sceneName)) continue;

            yield return UnloadSceneAsync(scene.sceneName);
        }

        onComplete?.Invoke();
    }

    private IEnumerator LoadWithLoadingScreen(
        List<SceneData> scenesToLoad,
        List<SceneData> scenesToUnload,
        Action onComplete = null)
    {
        // Show loading screen
        if (loadingScreenScene != null && !IsSceneLoaded(loadingScreenScene.sceneName))
        {
            yield return LoadSceneAsync(loadingScreenScene.sceneName);
        }

        // Allow loading screen to render
        yield return null;

        // Unload old scenes if provided
        if (scenesToUnload != null)
        {
            yield return UnloadScenesSequentially(scenesToUnload);
        }

        // Load new scenes
        if (scenesToLoad != null)
        {
            yield return LoadScenesSequentially(scenesToLoad);
        }

        // Hide loading screen
        if (loadingScreenScene != null && IsSceneLoaded(loadingScreenScene.sceneName))
        {
            yield return UnloadSceneAsync(loadingScreenScene.sceneName);
        }

        onComplete?.Invoke();
    }

    private IEnumerator TransitionSequentially(
        List<SceneData> scenesToUnload,
        List<SceneData> scenesToLoad,
        Action onComplete = null)
    {
        yield return UnloadScenesSequentially(scenesToUnload);
        yield return LoadScenesSequentially(scenesToLoad);
        onComplete?.Invoke();
    }

    // ──────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────

    private bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid() && scene.isLoaded;
    }

    private bool IsValidForCurrentBuild(SceneData sceneData)
    {
        if (!sceneData.debugOnly) return true;

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        return true;
        #else
        return false;
        #endif
    }

    private IEnumerator AwaitTask(Task task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
            Debug.LogException(task.Exception);
    }

    private async Task AwaitSceneObjectLoaders(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        var loaders = scene.GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<IAsyncSceneLoader>())
            .ToList();

        await Task.WhenAll(loaders.Select(l => l.LoadAsync()));
    }
}
