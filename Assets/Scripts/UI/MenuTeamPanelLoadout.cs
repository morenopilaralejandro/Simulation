using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Input;

/// <summary>
/// Displays all existing team loadouts and a "Create New" button.
/// </summary>
public class MenuTeamPanelLoadout : Menu
{
    #region Fields
    [Header("References")]
    [SerializeField] private ScrollViewAutoScroll autoScroll;
    [SerializeField] private Transform listRoot;
    [SerializeField] private LoadoutListItem itemPrefab;
    [SerializeField] private TMP_Text loadoutCountText;
    [SerializeField] private Button createButton;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isOnTop => menuManager.IsMenuOnTop(this);
    private List<LoadoutListItem> spawnedItems = new();
    private MenuManager menuManager;
    private TeamManager teamManager;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    private void Start()
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
        teamManager = TeamManager.Instance;
    }


    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        autoScroll.Activate();
        autoScroll.ResetToTop();

        Refresh();
    }

    public override void Hide()
    {
        autoScroll.Deactivate();

        base.SetInteractable(false);
        base.Hide();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);

        if (interactable)
            autoScroll.Activate();
        else
            autoScroll.Deactivate();

        if (interactable) 
            SubscribeInput();
        else
            UnsubscribeInput();
    }

    public void Close()
    {
        if (!isOpen) return;
        menuManager.CloseMenu();
        UIEvents.RaiseTeamMenuClosed();
    }

    #endregion

    #region Logic

    public void Refresh()
    {
        ClearList();

        List<Team> allLoadouts = teamManager.GetAllLoadouts();
        string activeGuid = teamManager.ActiveLoadoutGuid;

        foreach (Team loadout in allLoadouts)
        {
            LoadoutListItem item = Instantiate(itemPrefab, listRoot);
            bool isActive = loadout.TeamGuid == activeGuid;
            item.Initialize(loadout, isActive, HandleItemClicked);
            spawnedItems.Add(item);
        }

        base.SetDefaultSelectable(spawnedItems[0].GetComponent<Button>());

        UpdateCountText(allLoadouts.Count);
        UpdateCreateButtonState(allLoadouts.Count);
    }

    private void ClearList()
    {
        foreach (LoadoutListItem item in spawnedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }

    private void UpdateCountText(int count)
    {
        if (loadoutCountText != null)
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
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCloseClicked);
    }

    private void UnsubscribeInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCloseClicked);
    }

    #endregion

    #region Button Handlers

    private void HandleItemClicked(Team team)
    {
        UIEvents.RaiseTeamLoadoutSelected(team);
    }

    public void OnButtonCreateClicked()
    {
        UIEvents.RaiseTeamLoadoutCreateRequested();
    }

    public void OnButtonCloseClicked()
    {
        Close();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamLoadoutRequested += HandleRequested;
        UIEvents.OnTeamLoadoutCreateRequested += HandleCreateRequested;
        UIEvents.OnTeamLoadoutDeleteRequested += HandleDeleteRequested;
        TeamEvents.OnLoadoutDeleted += HandleLoadoutDeleted;
        TeamEvents.OnLoadoutUpdated += HandleLoadoutUpdated;
        UIEvents.OnBackFromTeamRequested += HandleBackToLoadoutList;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamLoadoutRequested -= HandleRequested;
        UIEvents.OnTeamLoadoutCreateRequested -= HandleCreateRequested;
        UIEvents.OnTeamLoadoutDeleteRequested -= HandleDeleteRequested;
        TeamEvents.OnLoadoutDeleted -= HandleLoadoutDeleted;
        TeamEvents.OnLoadoutUpdated -= HandleLoadoutUpdated;
        UIEvents.OnBackFromTeamRequested -= HandleBackToLoadoutList;
    }

    private void HandleRequested() 
    {
        menuManager.OpenMenu(this);
    }

    private void HandleCreateRequested() 
    {
        Team newLoadout = teamManager.CreateLoadout();
        if (newLoadout == null) return;
   
        // Refresh the list, then navigate into the new loadout
        Refresh();
        UIEvents.RaiseTeamLoadoutSelected(newLoadout);
    }

    private void HandleDeleteRequested(Team team)
    {
        teamManager.DeleteLoadout(team.TeamGuid);
        Refresh();
    }

    private void HandleLoadoutDeleted(Team team)
    {

    }

    private void HandleLoadoutUpdated(Team team)
    {
        Refresh();
    }

    private void HandleBackToLoadoutList(Team team, bool hasSwapped) 
    {
        Refresh();
    }

    #endregion
}
