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
    private FormationCharacterSlotUI currentSlot;
    private BattleType currentBattleType;
    private Character selectedCharacter;
    private Item selectedItem;

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

        selectedCharacter = null;
        selectedItem = null;
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

    public void OnButtonSelected(GameObject gameObject) 
    {
        UIEvents.RaiseTeamButtonSelected(gameObject);
    }

    public void OnButtonTeamActionsClicked() => UIEvents.RaiseTeamActionsOpened();

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamLoadoutSelected += HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUIClicked += HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISwaped += HandeFormationCharacterSlotUISwaped;
        UIEvents.OnFormationCharacterSlotUIReplaced += HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIReplaceRequested += HandleFormationCharacterSlotUIReplaceRequested;
        UIEvents.OnTeamButtonSelected += HandleTeamButtonSelected;
        UIEvents.OnCharacterSelected += HandleCharacterSelected;
        UIEvents.OnBattleTypeChangeRequested += HandleBattleTypeChangeRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamLoadoutSelected -= HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUIClicked -= HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISwaped -= HandeFormationCharacterSlotUISwaped;
        UIEvents.OnFormationCharacterSlotUIReplaced -= HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIReplaceRequested -= HandleFormationCharacterSlotUIReplaceRequested;
        UIEvents.OnTeamButtonSelected -= HandleTeamButtonSelected;
        UIEvents.OnCharacterSelected -= HandleCharacterSelected;
        UIEvents.OnBattleTypeChangeRequested -= HandleBattleTypeChangeRequested;
    }

    private void HandleLoadoutSelected(Team team) 
    {
        currentTeam = team;
        menuManager.OpenMenu(this);
    }

    private void HandleFormationCharacterSlotUIClicked(FormationCharacterSlotUI slot) 
    {
        if (!isEditMode) return;
        currentSlot = slot;
        UIEvents.RaiseCharacterActionsOpened();
    }

    private void HandeFormationCharacterSlotUISwaped(FormationCharacterSlotUI a, FormationCharacterSlotUI b) 
    {
        // character where already swaped on the slot script

        teamManager.SetCharacterInLoadout(
            currentTeam.TeamGuid,
            currentBattleType,
            a.SlotIndex,
            a.GetCharacter().CharacterGuid
        );

        teamManager.SetCharacterInLoadout(
            currentTeam.TeamGuid,
            currentBattleType,
            b.SlotIndex,
            b.GetCharacter().CharacterGuid
        );
    }

    private void HandleFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character)
    {
        teamManager.SetCharacterInLoadout(
            currentTeam.TeamGuid,
            currentBattleType,
            currentSlot.SlotIndex,
            character.CharacterGuid
        );
    }

    private void HandleFormationCharacterSlotUIReplaceRequested() 
    {
        if (selectedCharacter == null) return;
        UIEvents.RaiseFormationCharacterSlotUIReplaced(currentSlot, selectedCharacter);
    }

    private void HandleTeamButtonSelected(GameObject gameObject) 
    {
        base.SetLastSelected(gameObject);
    }

    private void HandleCharacterSelected(Character character) 
    {
        selectedCharacter = character;
    }

    public void HandleBattleTypeChangeRequested()
    {
        if(!isEditMode) return;

        BattleType newType = currentBattleType == BattleType.Full
            ? BattleType.Mini
            : BattleType.Full;

        formationLayoutUI.Initialize(currentTeam, currentBattleType);
        UIEvents.RaiseBattleTypeChanged(newType, currentBattleType);
    }

    #endregion
}
