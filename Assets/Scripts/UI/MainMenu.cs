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

    private void Start() 
    {
        AudioManager.Instance.PlayBgm("bgm-simulation");    
        panelMain.SetActive(true);
        panelSettings.SetActive(false);
        panelCredits.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    public void OnButtonDreamMatchTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        BattleArgs.TeamId0 = "faith_selection";
        BattleArgs.TeamId1 = "crimson_selection";
        BattleArgs.BallId = "crimson";
        BattleArgs.FieldId = "stadium_main";
        BattleArgs.BattleType = BattleType.Battle;

        SceneLoader.UnloadMainMenu();
        SceneLoader.LoadBattle();
    }

    public void OnButtonSettingsTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        panelMain.SetActive(false);
        panelSettings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
    }

    public void OnButtonSettingsContinueTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_back");
        panelSettings.SetActive(false);
        panelMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    public void OnButtonCreditsTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        panelMain.SetActive(false);
        panelCredits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedCredits);
    }

    public void OnButtonCreditsContinueTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_back");
        panelCredits.SetActive(false);
        panelMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    public void OnButtonQuitTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        Application.Quit();
    }

}
