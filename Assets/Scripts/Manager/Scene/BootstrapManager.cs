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

        AsyncOperationHandle initAddressablesHandle = Addressables.InitializeAsync();
        yield return initAddressablesHandle;

        yield return new WaitUntil(() => CharacterManager.Instance != null);
        yield return new WaitUntil(() => CharacterManager.Instance.IsReady);

        SceneManager.UnloadSceneAsync(LoadingData.LoadingSceneName);

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        SceneLoader.LoadDebugMainMenu();
    #else
        SceneLoader.LoadMainMenu();
    #endif
    }
}

