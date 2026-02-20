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
    private InputManager inputManager;

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
        inputManager = InputManager.Instance;
    }

    // ──────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────

    public void LoadScene(SceneData sceneData, Action onComplete = null)
    {
        if (sceneData == null)
        {
            LogManager.Error("[SceneLoader] Attempted to load a null SceneData.");
            return;
        }

        if (!IsValidForCurrentBuild(sceneData))
        {
            LogManager.Warning($"[SceneLoader] Scene '{sceneData.sceneName}' is debug-only and excluded from this build.");
            onComplete?.Invoke();
            return;
        }

        if (IsSceneLoaded(sceneData.sceneName))
        {
            LogManager.Warning($"[SceneLoader] Scene '{sceneData.sceneName}' is already loaded.");
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

    public void UnloadScene(SceneData sceneData, Action onComplete = null)
    {
        if (sceneData == null)
        {
            LogManager.Error("[SceneLoader] Attempted to unload a null SceneData.");
            return;
        }

        if (!IsSceneLoaded(sceneData.sceneName))
        {
            LogManager.Warning($"[SceneLoader] Scene '{sceneData.sceneName}' is not loaded.");
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(UnloadSceneAsync(sceneData.sceneName, onComplete));
    }

    public void LoadGroup(SceneGroup group, Action onComplete = null)
    {
        if (group == null)
        {
            LogManager.Error("[SceneLoader] Attempted to load a null SceneGroup.");
            return;
        }

        List<SceneData> validScenes = group.GetValidScenes();

        if (validScenes.Count == 0)
        {
            LogManager.Warning($"[SceneLoader] Group '{group.groupName}' has no valid scenes.");
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

    public void UnloadGroup(SceneGroup group, Action onComplete = null)
    {
        if (group == null)
        {
            LogManager.Error("[SceneLoader] Attempted to unload a null SceneGroup.");
            return;
        }

        List<SceneData> validScenes = group.GetValidScenes();
        StartCoroutine(UnloadScenesSequentially(validScenes, onComplete));
    }

    public void LoadGroupByName(string groupName, Action onComplete = null)
    {
        SceneGroup group = registry.GetGroupByName(groupName);
        if (group != null)
        {
            LoadGroup(group, onComplete);
        }
    }

    public void UnloadGroupByName(string groupName, Action onComplete = null)
    {
        SceneGroup group = registry.GetGroupByName(groupName);
        if (group != null)
        {
            UnloadGroup(group, onComplete);
        }
    }

    public void TransitionGroups(SceneGroup fromGroup, SceneGroup toGroup, Action onComplete = null)
    {
        if (fromGroup == null || toGroup == null)
        {
            LogManager.Error("[SceneLoader] TransitionGroups requires both a 'from' and 'to' group.");
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
    // Guarded Coroutine Wrapper
    // ──────────────────────────────────────────────

    /// <summary>
    /// Wraps any transition coroutine with input locking.
    /// Guarantees UnlockInput is called even if the inner coroutine fails.
    /// </summary>
    private IEnumerator WithInputLock(IEnumerator innerCoroutine, Action onComplete = null)
    {
        inputManager?.LockInput();

        Exception caughtException = null;

        // Run the inner coroutine, catching any exception
        while (true)
        {
            bool hasNext;
            try
            {
                hasNext = innerCoroutine.MoveNext();
            }
            catch (Exception ex)
            {
                caughtException = ex;
                break;
            }

            if (!hasNext) break;
            yield return innerCoroutine.Current;
        }

        inputManager?.UnlockInput();

        if (caughtException != null)
        {
            LogManager.Error($"[SceneLoader] Exception during scene transition: {caughtException}");
        }

        onComplete?.Invoke();
    }

    // ──────────────────────────────────────────────
    // Core Coroutines (no input lock — wrapped above)
    // ──────────────────────────────────────────────

    private IEnumerator LoadSceneAsync(string sceneName, Action onComplete = null)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (operation == null)
        {
            LogManager.Error($"[SceneLoader] Failed to start loading scene '{sceneName}'.");
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
            LogManager.Error($"[SceneLoader] Failed to start unloading scene '{sceneName}'.");
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

    // ──────────────────────────────────────────────
    // Transition Coroutines (WITH input locking)
    // ──────────────────────────────────────────────

    private IEnumerator LoadWithLoadingScreen(
        List<SceneData> scenesToLoad,
        List<SceneData> scenesToUnload,
        Action onComplete = null)
    {
        // ★ Lock input for the entire loading screen transition
        yield return WithInputLock(
            LoadWithLoadingScreenInner(scenesToLoad, scenesToUnload),
            onComplete
        );
    }

    private IEnumerator LoadWithLoadingScreenInner(
        List<SceneData> scenesToLoad,
        List<SceneData> scenesToUnload)
    {
        // Show loading screen
        if (loadingScreenScene != null && !IsSceneLoaded(loadingScreenScene.sceneName))
        {
            yield return LoadSceneAsync(loadingScreenScene.sceneName);
        }

        yield return null; // Let loading screen render

        // Unload old scenes
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
    }

    private IEnumerator TransitionSequentially(
        List<SceneData> scenesToUnload,
        List<SceneData> scenesToLoad,
        Action onComplete = null)
    {
        // ★ Lock input for the entire sequential transition
        yield return WithInputLock(
            TransitionSequentiallyInner(scenesToUnload, scenesToLoad),
            onComplete
        );
    }

    private IEnumerator TransitionSequentiallyInner(
        List<SceneData> scenesToUnload,
        List<SceneData> scenesToLoad)
    {
        yield return UnloadScenesSequentially(scenesToUnload);
        yield return LoadScenesSequentially(scenesToLoad);
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
