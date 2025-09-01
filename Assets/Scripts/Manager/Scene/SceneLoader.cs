using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static void LoadMainMenu() {
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("GlobalLighting", LoadSceneMode.Additive);
    }

    public static void UnloadMainMenu() {
        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }

    public static void LoadBattle()
    {
        // Environment
        SceneManager.LoadSceneAsync("BattleMap", LoadSceneMode.Additive);
        // Spawners
        SceneManager.LoadSceneAsync("BattleSpawners", LoadSceneMode.Additive);
        // UI
        SceneManager.LoadSceneAsync("BattleUI", LoadSceneMode.Additive);
        // Lighting
        SceneManager.LoadSceneAsync("GlobalLighting", LoadSceneMode.Additive);
    }

    public static void UnloadBattle()
    {
        SceneManager.UnloadSceneAsync("Stadium_Main");
        SceneManager.UnloadSceneAsync("Spawners");
        SceneManager.UnloadSceneAsync("BattleUI");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }
}

