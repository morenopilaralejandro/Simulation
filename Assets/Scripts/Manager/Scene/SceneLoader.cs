using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static void LoadLoadingScreen() {
        SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
    }

    public static void UnloadLoadingScreen() {
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }



    public static void LoadSystemManager() {
        SceneManager.LoadSceneAsync("SystemManager", LoadSceneMode.Additive);
    }

    public static void UnloadSystemManager() {
        SceneManager.UnloadSceneAsync("SystemManager");
    }



    public static void LoadMainCamera() {
        SceneManager.LoadSceneAsync("MainCamera", LoadSceneMode.Additive);
    }

    public static void UnloadMainCamera() {
        SceneManager.UnloadSceneAsync("MainCamera");
    }



    public static void LoadDebugMainMenu() {
        LoadingScreen.LoadScenes(new string[] { "DebugMainMenu", "GlobalLighting"});
    }

    public static void UnloadDebugMainMenu() {
        SceneManager.UnloadSceneAsync("DebugMainMenu");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }



    public static void LoadMainMenu() {
        LoadingScreen.LoadScenes(new string[] { "MainMenu", "GlobalLighting"});
    }

    public static void UnloadMainMenu() {
        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }



    public static void LoadBattleResults() {
        LoadingScreen.LoadScenes(new string[] { "BattleResults", "GlobalLighting"});
    }

    public static void UnloadBattleResults() {
        SceneManager.UnloadSceneAsync("BattleResults");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }



    public static void LoadGameOver() {
        LoadingScreen.LoadScenes(new string[] { "GameOver", "GlobalLighting"});
    }

    public static void UnloadGameOver() {
        SceneManager.UnloadSceneAsync("GameOver");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }



    public static void LoadBattle()
    {
        LoadingScreen.LoadScenes(new string[] { "BattleMap", "BattleUI", "BattleSpawners", "GlobalLighting", "BattleCamera"});
    }

    public static void UnloadBattle()
    {
        SceneManager.UnloadSceneAsync("BattleUI");
        SceneManager.UnloadSceneAsync("BattleMap");
        SceneManager.UnloadSceneAsync("BattleCamera");
        SceneManager.UnloadSceneAsync("BattleSpawners");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }
}

