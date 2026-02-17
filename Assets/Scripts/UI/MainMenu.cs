using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Simulation.Enums.Battle;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject panelCredits;
    [SerializeField] private GameObject firstSelectedMain;
    [SerializeField] private GameObject firstSelectedSettings;
    [SerializeField] private GameObject firstSelectedCredits;

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneBattle;
    [SerializeField] private SceneGroup sceneMainMenu;
    private SceneLoader sceneLoader;
    private AudioManager audioManager;

    private void Start() 
    {
        sceneLoader = SceneLoader.Instance;
        audioManager = AudioManager.Instance;

        audioManager.PlayBgm("bgm-simulation");    
        panelMain.SetActive(true);
        panelSettings.SetActive(false);
        panelCredits.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    public void OnButtonDreamMatchTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        BattleArgs.SetFull(
            "faith_selection", 
            "crimson_selection");
        sceneLoader.LoadGroup(sceneBattle);
    }

    public void OnButtonSettingsTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        panelMain.SetActive(false);
        panelSettings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
    }

    public void OnButtonSettingsContinueTapped() 
    {
        audioManager.PlaySfx("sfx-menu_back");
        panelSettings.SetActive(false);
        panelMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    public void OnButtonCreditsTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        panelMain.SetActive(false);
        panelCredits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedCredits);
    }

    public void OnButtonCreditsContinueTapped() 
    {
        audioManager.PlaySfx("sfx-menu_back");
        panelCredits.SetActive(false);
        panelMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    public void OnButtonQuitTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        Application.Quit();
    }

}
