using System;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Input;

public class MenuQuitOverworld : Menu
{
    [Header("UI References")]
    [SerializeField] private SceneGroup sceneMainMenu;

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    public async void OnButtonConfirmClicked()
    {
        bool unloadSuccess = await WorldManager.Instance.UnloadCurrentZone();
        SceneLoader.Instance.LoadGroup(sceneMainMenu);
    }

    public void OnButtonCancelClicked()
    {
        RequestClose();
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
