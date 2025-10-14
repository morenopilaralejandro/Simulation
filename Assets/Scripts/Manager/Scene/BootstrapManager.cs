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

        yield return new WaitUntil(() => CharacterManager.Instance != null);
        yield return new WaitUntil(() => CharacterManager.Instance.IsReady);

        yield return new WaitUntil(() => MoveManager.Instance != null);
        yield return new WaitUntil(() => MoveManager.Instance.IsReady);

        yield return new WaitUntil(() => MoveEvolutionGrowthProfileManager.Instance != null);
        yield return new WaitUntil(() => MoveEvolutionGrowthProfileManager.Instance.IsReady);

        yield return new WaitUntil(() => MoveEvolutionPathManager.Instance != null);
        yield return new WaitUntil(() => MoveEvolutionPathManager.Instance.IsReady);

        yield return new WaitUntil(() => FormationCoordManager.Instance != null);
        yield return new WaitUntil(() => FormationCoordManager.Instance.IsReady);

        yield return new WaitUntil(() => FormationManager.Instance != null);
        yield return new WaitUntil(() => FormationManager.Instance.IsReady);

        yield return new WaitUntil(() => KitManager.Instance != null);
        yield return new WaitUntil(() => KitManager.Instance.IsReady);

        yield return new WaitUntil(() => TeamManager.Instance != null);
        yield return new WaitUntil(() => TeamManager.Instance.IsReady);

        SceneManager.UnloadSceneAsync(LoadingData.LoadingSceneName);

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        SceneLoader.LoadDebugMainMenu();
    #else
        SceneLoader.LoadMainMenu();
    #endif
    }
}

