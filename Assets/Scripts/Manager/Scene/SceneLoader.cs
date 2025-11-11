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



    public static void LoadGlobalLighting() {
        SceneManager.LoadSceneAsync("GlobalLighting", LoadSceneMode.Additive);
    }

    public static void UnloadGlobalLighting() {
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }



    public static void LoadDebugMainMenu() {
        LoadingScreen.LoadScenes(new string[] { "DebugMainMenu" });
    }

    public static void UnloadDebugMainMenu() {
        SceneManager.UnloadSceneAsync("DebugMainMenu");
    }



    public static void LoadMainMenu() {
        LoadingScreen.LoadScenes(new string[] { "MainMenu" });
    }

    public static void UnloadMainMenu() {
        SceneManager.UnloadSceneAsync("MainMenu");
    }



    public static void LoadBattleResults() {
        LoadingScreen.LoadScenes(new string[] { "BattleResults" });
    }

    public static void UnloadBattleResults() {
        SceneManager.UnloadSceneAsync("BattleResults");
    }



    public static void LoadGameOver() {
        LoadingScreen.LoadScenes(new string[] { "GameOver" });
    }

    public static void UnloadGameOver() {
        SceneManager.UnloadSceneAsync("GameOver");
    }



    public static void LoadBattle()
    {
        LoadingScreen.LoadScenes(new string[] { 
            "BattleMap", 
            "BattleUI", 
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            "DebugBattleUI", 
        #endif
            "BattleSpawners", 
            "BattleCamera"
        });
    }

    public static void UnloadBattle()
    {
        SceneManager.UnloadSceneAsync("BattleUI");
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        SceneManager.UnloadSceneAsync("DebugBattleUI");
    #endif
        SceneManager.UnloadSceneAsync("BattleMap");
        SceneManager.UnloadSceneAsync("BattleCamera");
        SceneManager.UnloadSceneAsync("BattleSpawners");
    }
}

