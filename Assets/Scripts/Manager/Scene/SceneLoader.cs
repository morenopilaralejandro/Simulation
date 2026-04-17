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

    // ★ NEW: Global loading guard
    private bool isLoading = false;
    public bool IsLoading => isLoading;

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

        if (isLoading)
        {
            LogManager.Warning("[SceneLoader] Already loading. Ignoring LoadScene request.");
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

        if (isLoading)
        {
            LogManager.Warning("[SceneLoader] Already loading. Ignoring UnloadScene request.");
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

        if (isLoading)
        {
            LogManager.Warning($"[SceneLoader] Already loading. Ignoring LoadGroup '{group.groupName}'.");
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

        if (isLoading)
        {
            LogManager.Warning("[SceneLoader] Already loading. Ignoring UnloadGroup request.");
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

        if (isLoading)
        {
            LogManager.Warning("[SceneLoader] Already loading. Ignoring TransitionGroups request.");
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

    private IEnumerator WithInputLock(IEnumerator innerCoroutine, Action onComplete = null)
    {
        // ★ SET FLAG IMMEDIATELY (same frame as call)
        isLoading = true;
        inputManager?.LockInput();

        Exception caughtException = null;

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
        // ★ CLEAR FLAG when done
        isLoading = false;

        if (caughtException != null)
        {
            LogManager.Error($"[SceneLoader] Exception during scene transition: {caughtException}");
        }

        onComplete?.Invoke();
    }

    // ──────────────────────────────────────────────
    // Core Coroutines
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
    // Transition Coroutines
    // ──────────────────────────────────────────────

    private IEnumerator LoadWithLoadingScreen(
        List<SceneData> scenesToLoad,
        List<SceneData> scenesToUnload,
        Action onComplete = null)
    {
        yield return WithInputLock(
            LoadWithLoadingScreenInner(scenesToLoad, scenesToUnload),
            onComplete
        );
    }

    private IEnumerator LoadWithLoadingScreenInner(
        List<SceneData> scenesToLoad,
        List<SceneData> scenesToUnload)
    {
        if (loadingScreenScene != null && !IsSceneLoaded(loadingScreenScene.sceneName))
        {
            yield return LoadSceneAsync(loadingScreenScene.sceneName);
        }

        yield return null;

        if (scenesToUnload != null)
        {
            yield return UnloadScenesSequentially(scenesToUnload);
        }

        if (scenesToLoad != null)
        {
            yield return LoadScenesSequentially(scenesToLoad);
        }

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

    public IEnumerator AwaitTask(Task task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
            Debug.LogException(task.Exception);
    }

    public async Task AwaitSceneObjectLoaders(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        var loaders = scene.GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<IAsyncSceneLoader>())
            .ToList();

        LogManager.Trace($"[SceneLoader] [{sceneName}] Found {loaders.Count} IAsyncSceneLoader(s):");
        await Task.WhenAll(loaders.Select(l => l.LoadAsync()));
    }
}
