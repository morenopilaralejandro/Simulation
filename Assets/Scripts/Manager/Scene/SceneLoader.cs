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



    public static void LoadBattle()
    {
        LoadingScreen.LoadScenes(new string[] { "BattleMap", "BattleUI", "BattleSpawners", "GlobalLighting"});
    }

    public static void UnloadBattle()
    {
        SceneManager.UnloadSceneAsync("BattleMap");
        SceneManager.UnloadSceneAsync("BattleSpawners");
        SceneManager.UnloadSceneAsync("BattleUI");
        SceneManager.UnloadSceneAsync("GlobalLighting");
    }
}

