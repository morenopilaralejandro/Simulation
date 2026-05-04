using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Input;

public class MenuTeamPanelTeamActions : Menu
{
    private Team team;
    private BattleType battleType;

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonChangeFormationClicked() 
    {
        UIEvents.RaiseItemSelectorSideOpened(ItemCategory.Formation, battleType);
    }

    public void OnButtonChangeKitClicked() 
    {
        UIEvents.RaiseItemSelectorSideOpened(ItemCategory.Kit, battleType);
    }

    public void OnButtonChangeNameClicked() 
    {
        UIEvents.RaiseTeamPanelNameOpened(team.TeamName);
    }

    public void OnButtonChangeEmblemClicked() 
    {
        UIEvents.RaiseTeamPanelEmblemOpened(team.TeamCrestSprite);
    }

    public void OnButtonChangeBattleTypeClicked() 
    {
        UIEvents.RaiseBattleTypeChangeRequested();
    }

    public void OnButtonBackClicked() 
    {
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamActionsOpened += HandleTeamActionsOpened;
        UIEvents.OnBackFromTeamActionsRequested += HandleBackFromTeamActionsRequested;
        UIEvents.OnBattleTypeChanged += HandleBattleTypeChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamActionsOpened -= HandleTeamActionsOpened;
        UIEvents.OnBackFromTeamActionsRequested -= HandleBackFromTeamActionsRequested;
        UIEvents.OnBattleTypeChanged -= HandleBattleTypeChanged;
    }

    private void HandleTeamActionsOpened(Team team, BattleType battleType) 
    {
        this.team = team;
        this.battleType = battleType;
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleBackFromTeamActionsRequested() 
    {
        RequestClose();
    }

    private void HandleBattleTypeChanged(BattleType currentBattleType, BattleType oldType) 
    {
        this.battleType = currentBattleType;
    }

}
