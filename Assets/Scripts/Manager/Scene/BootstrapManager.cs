using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class BootstrapManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadSystemAndInitialScenes());
    }

    private IEnumerator LoadSystemAndInitialScenes()
    {
        SceneManager.LoadScene(LoadingData.LoadingSceneName);

        SceneLoader.LoadSystemManager();
        SceneLoader.LoadMainCamera();

        AsyncOperationHandle initAddressablesHandle = Addressables.InitializeAsync();
        yield return initAddressablesHandle;

        yield return new WaitUntil(() => DataLoadManager.Instance != null);
        yield return new WaitUntil(() => DataLoadManager.Instance.IsReady);

        SceneManager.UnloadSceneAsync(LoadingData.LoadingSceneName);

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        SceneLoader.LoadDebugMainMenu();
    #else
        SceneLoader.LoadMainMenu();
    #endif
    }
}

