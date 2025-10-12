using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

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
        yield return null;

        for (int i = 0; i < scenes.Length; i++)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);

            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                UpdateLoadingUI(progress);

                if (asyncLoad.progress >= 0.9f)
                    asyncLoad.allowSceneActivation = true;

                yield return null;
            }
        }
        SceneManager.UnloadSceneAsync(LoadingData.LoadingSceneName);
    }

    private void UpdateLoadingUI(float progress)
    {

    }
}
