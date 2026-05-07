using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Aremoreno.Enums.Input;

/// <summary>
/// Displays all existing team loadouts and a "Create New" button.
/// </summary>
public class MenuTeamPanelLoadout : SelectorLoadout
{
    #region Fields

    [Header("References")]
    [SerializeField] private TMP_Text loadoutCountText;
    [SerializeField] private Button createButton;

    #endregion

    #region Menu Overrides

    public void RefreshInternal()
    {
        base.Refresh();

        int count = TeamManager.Instance.GetLoadoutCount();
        UpdateCountText(count);
        UpdateCreateButtonState(count);
    }

    protected override void OnGainedInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    protected override void OnLostInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    #endregion

    #region Input

    private void HandleBack()
    {
        UIEvents.RaiseTeamMenuClosed();
        RequestClose();
    }

    #endregion

    #region Logic
 
    private void UpdateCountText(int count)
    {
        loadoutCountText.text = $"({count} / {TeamManager.MAX_LOADOUTS})";
    }

    private void UpdateCreateButtonState(int count)
    {
        createButton.interactable = count < TeamManager.MAX_LOADOUTS;
    }

    #endregion

    #region Input

    private void SubscribeInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    private void UnsubscribeInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    public void OnButtonBackClicked() => HandleBack();

    public void OnButtonCreateClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        UIEvents.RaiseTeamLoadoutCreateRequested();
    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnLoadoutSelectorOpenRequested += HandleOpenRequested;
        UIEvents.OnTeamLoadoutCreateRequested += HandleCreateRequested;
        UIEvents.OnTeamLoadoutDeleteRequested += HandleDeleteRequested;
        TeamEvents.OnLoadoutDeleted += HandleLoadoutDeleted;
        TeamEvents.OnLoadoutUpdated += HandleLoadoutUpdated;
        UIEvents.OnBackFromTeamRequested += HandleBackToLoadoutList;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnLoadoutSelectorOpenRequested -= HandleOpenRequested;
        UIEvents.OnTeamLoadoutCreateRequested -= HandleCreateRequested;
        UIEvents.OnTeamLoadoutDeleteRequested -= HandleDeleteRequested;
        TeamEvents.OnLoadoutDeleted -= HandleLoadoutDeleted;
        TeamEvents.OnLoadoutUpdated -= HandleLoadoutUpdated;
        UIEvents.OnBackFromTeamRequested -= HandleBackToLoadoutList;
    }

    private void HandleOpenRequested(
        ISelectorSource<Team>      source,
        ISelectorClickAction<Team> action,
        ISelectorFilter<Team>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        Open(source, action, filter);
    }

    private void HandleCreateRequested() 
    {
        Team newLoadout = TeamManager.Instance.CreateLoadout();
        if (newLoadout == null) return;
   
        // Refresh the list, then navigate into the new loadout
        RefreshInternal();
        UIEvents.RaiseSelectorLoadoutActionClicked(newLoadout);
    }

    private void HandleDeleteRequested(Team team)
    {
        TeamManager.Instance.DeleteLoadout(team.TeamGuid);
        RefreshInternal();
    }

    private void HandleLoadoutDeleted(Team team)
    {

    }

    private void HandleLoadoutUpdated(Team team)
    {
        RefreshInternal();
    }

    private void HandleBackToLoadoutList(Team team, bool hasSwapped) 
    {
        RefreshInternal();
    }

    #endregion
}
