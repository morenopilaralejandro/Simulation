using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuTeamPanelDelete : Menu
{
    private Team team;

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    public void OnButtonConfirmClicked()
    {
        UIEvents.RaiseTeamLoadoutDeleteRequested(team);
        RequestClose();
    }

    public void OnButtonCancelClicked()
    {
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamPanelDeleteOpened += HandleTeamPanelDeleteOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamPanelDeleteOpened -= HandleTeamPanelDeleteOpened;
    }

    private void HandleTeamPanelDeleteOpened(Team team)
    {
        this.team = team;
        MenuManager.Instance.OpenMenu(this);
    }
}
