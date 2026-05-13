using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuMainPanelStory : Menu
{
    [Header("UI References")]
    [SerializeField] private Menu menuStoryConfirm;
    [SerializeField] private SceneGroup sceneWorld;
    [SerializeField] private SaveFileCard saveFileCard;
    [SerializeField] private Button buttonContinueGame;
    [SerializeField] private Button buttonNewGame;
    private bool hasSaveData;

    public override void Show() 
    {
        hasSaveData = PersistenceManager.Instance.HasSaveData();
        buttonContinueGame.interactable = hasSaveData;

        base.Show();

        if(!hasSaveData) 
        {
            SetDefaultSelectable(buttonNewGame);
        }
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonBackClicked()
    {
        RequestClose();
    }

    public void OnButtonContinueClicked() 
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        ContinueGame();
    }

    public void OnButtonNewGameClicked() 
    {
        if (!hasSaveData) 
        {
            StartNewGame();
            return;
        }

        MenuManager.Instance.OpenMenu(menuStoryConfirm);
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

    private void StartNewGame() 
    {
        PersistenceManager.Instance.StartNewGame();
        SceneLoader.Instance.LoadGroup(sceneWorld);
    }

    private void ContinueGame() 
    {
        PersistenceManager.Instance.LoadGame();
        SceneLoader.Instance.LoadGroup(sceneWorld);
    }

}
