using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private SceneGroup sceneMainMenu;
    [SerializeField] private SceneGroup sceneDebugMainMenu;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadSystemAndInitialScenes());
    }

    private IEnumerator LoadSystemAndInitialScenes()
    {
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);

        SceneManager.LoadSceneAsync("MainCamera", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("SystemManager", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("GlobalLighting", LoadSceneMode.Additive);

        AsyncOperationHandle initAddressablesHandle = Addressables.InitializeAsync();
        yield return initAddressablesHandle;

        yield return new WaitUntil(() => DataLoadManager.Instance != null);
        yield return new WaitUntil(() => DataLoadManager.Instance.IsReady);

        SceneManager.UnloadSceneAsync("LoadingScene");

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        //SceneLoader.Instance.LoadGroup(sceneMainMenu);
        SceneLoader.Instance.LoadGroup(sceneDebugMainMenu);
    #else
        SceneLoader.Instance.LoadGroup(sceneMainMenu);
        //SceneLoader.Instance.LoadGroup(sceneDebugMainMenu);
    #endif
    }

}

