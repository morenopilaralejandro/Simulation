using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LoadingScreen : MonoBehaviour
{
    public static void LoadScenes(string[] scenes)
    {
        SceneManager.LoadSceneAsync(LoadingData.LoadingSceneName, LoadSceneMode.Additive);
        LoadingData.ScenesToLoad = scenes;      
    }

    private void Start()
    {
        if (LoadingData.ScenesToLoad != null)
        {
            StartCoroutine(LoadScenesAdditively(LoadingData.ScenesToLoad));
        }
    }

    private IEnumerator LoadScenesAdditively(string[] scenes)
    {
        InputManager.Instance.LockInput();
        yield return null;

        for (int i = 0; i < scenes.Length; i++)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                UpdateLoadingUI(asyncLoad.progress / 0.9f);
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
                yield return null;

            yield return AwaitTask(AwaitSceneObjectLoaders(scenes[i]));
        }

        InputManager.Instance.UnlockInput();
        SceneManager.UnloadSceneAsync(LoadingData.LoadingSceneName);
    }

    private async Task AwaitSceneObjectLoaders(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        var loaders = scene.GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<IAsyncSceneLoader>())
            .ToList();

        await Task.WhenAll(loaders.Select(l => l.LoadAsync()));
    }

    private IEnumerator AwaitTask(Task task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
            Debug.LogException(task.Exception);
    }

    private void UpdateLoadingUI(float progress)
    {

    }

    public static bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
                return true;
        }
        return false;
    }
}
