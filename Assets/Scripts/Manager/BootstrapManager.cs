using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private string systemScene = "SystemManager";   // your core systems

    private void Awake()
    {
        // Ensure bootstrap itself never dies
        DontDestroyOnLoad(gameObject);

        // Kick off bootstrap process
        StartCoroutine(LoadSystemAndInitialScenes());
    }

    private IEnumerator LoadSystemAndInitialScenes()
    {
        // 1. Load SystemManager (contains GameManager, CharacterManager, etc.)
        AsyncOperation sysLoad = SceneManager.LoadSceneAsync(systemScene, LoadSceneMode.Additive);
        yield return sysLoad;

        Debug.Log("SystemManager loaded.");

        SceneLoader.LoadMainMenu();
    }
}

