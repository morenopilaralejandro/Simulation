using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuMain : Menu
{
    [Header("UI References")]
    [SerializeField] private Menu menuSettings;
    [SerializeField] private Menu menuCredits;
    [SerializeField] private Menu menuStory;

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneBattle;

    public void Start() 
    {
        MenuManager.Instance.OpenMenu(this);
        AudioManager.Instance.PlayBgm("bgm-simulation");    
    }

    public void OnButtonDreamMatchTapped() 
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");    
        BattleArgs.SetFull(
            "faith_selection", 
            "crimson_selection");
        SceneLoader.Instance.LoadGroup(sceneBattle);
    }

    public void OnButtonSettingsTapped() 
    {
        MenuManager.Instance.OpenMenu(menuSettings);
    }

    public void OnButtonCreditsTapped() 
    {
        MenuManager.Instance.OpenMenu(menuCredits);
    }

    public void OnButtonQuitTapped() 
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");    
        Application.Quit();
    }

    public void OnButtonStoryTapped() 
    {
        MenuManager.Instance.OpenMenu(menuStory);
    }

    /*
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamPanelNameOpened += HandleTeamPanelNameOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamPanelNameOpened -= HandleTeamPanelNameOpened;
    }

    private void HandleTeamPanelNameOpened(string teamName)
    {
        inputFieldName.text = teamName;
        MenuManager.Instance.OpenMenu(this);
    }
    */
}
