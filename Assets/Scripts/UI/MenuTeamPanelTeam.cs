using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;
using Simulation.Enums.UI;

/// <summary>
/// Displays the selected team's formation and characters for a given BattleType.
/// </summary>
public class MenuTeamPanelTeam : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private MenuTeamMode mode;
    [SerializeField] private Image activeImage;

    [SerializeField] private FormationLayoutUI formationLayoutUI;

    /*
    [SerializeField] private MenuTeamPanelTeamActions panelTeamActions;
    [SerializeField] private MenuTeamPanelSettings panelSettings;    
    [SerializeField] private MenuTeamPanelCharacterActions panelCharacterActions;
    [SerializeField] private MenuCharacterDetail menuCharacterDetail;
    [SerializeField] private MenuCharacterSelector menuCharacterSelector;
    [SerializeField] private MenuItemSelectorSide menuItemSelectorSide;
    */

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private MenuManager menuManager;
    private TeamManager teamManager;

    private Team currentTeam;
    private GameObject selectedSlot;
    private BattleType currentBattleType;

    

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        currentBattleType = BattleType.Full;
        menuManager = MenuManager.Instance;
        teamManager = TeamManager.Instance;
    }

    private void OnDestroy()
    {

    }

    #endregion

   #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        PopulateUI();
    }

    public override void Hide()
    {
        base.SetInteractable(false);
        base.Hide();

        currentTeam = null;
    }

    public void Close()
    {
        if (!isOpen) return;
        if(!isEditMode) return;
        UIEvents.RaiseBackFromTeamRequested();
        menuManager.CloseMenu();
    }

    public void Refresh()
    {
        PopulateUI();
    }

    #endregion

    #region Populate

    private void PopulateUI()
    {
        if (currentTeam == null) return;
        UpdateSetActiveButtonState();
        formationLayoutUI.Initialize(currentTeam, currentBattleType);
    }

    #endregion

    #region Helper

    private bool isEditMode => mode == MenuTeamMode.Edit;
    private bool isBattleMode => mode == MenuTeamMode.Battle;

    private void UpdateSetActiveButtonState() 
    {
        activeImage.enabled = teamManager.ActiveLoadoutGuid == currentTeam.TeamGuid;
    }

    #endregion

    #region Button Handle

    public void OnButtonToggleBattleTypeClicked()
    {
        if(!isEditMode) return;

        BattleType newType = currentBattleType == BattleType.Full
            ? BattleType.Mini
            : BattleType.Full;

        UIEvents.RaiseBattleTypeChanged(newType, currentBattleType);
    }

    public void OnButtonSetActiveClicked()
    {
        if(!isEditMode) return;
   
        if(teamManager.ActiveLoadoutGuid == currentTeam.TeamGuid) return;
        teamManager.SetActiveLoadout(currentTeam.TeamGuid);
        
        UpdateSetActiveButtonState();
    }

    public void OnButtonCloseClicked()
    {
        Close();     
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamLoadoutSelected += HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUIClicked += HandleFormationCharacterSlotUIClicked;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamLoadoutSelected -= HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUIClicked -= HandleFormationCharacterSlotUIClicked;
    }

    private void HandleLoadoutSelected(Team team) 
    {
        currentTeam = team;
        menuManager.OpenMenu(this);
    }

    private void HandleFormationCharacterSlotUIClicked(Character character) 
    {
        if (!isEditMode) return;
        //TODO open the character actions
    }

    #endregion
}
