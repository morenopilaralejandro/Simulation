using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;
using Simulation.Enums.UI;
using Simulation.Enums.Item;

/// <summary>
/// Displays the selected team's formation and characters for a given BattleType.
/// </summary>
public class MenuTeamPanelTeam : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private MenuTeamMode mode;
    [SerializeField] private Image activeImage;

    [SerializeField] private GameObject panelActive;
    [SerializeField] private GameObject panelChanges;
    [SerializeField] private GameObject panelTeamButtons;

    [SerializeField] private TMP_Text textChanges;

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
    private ItemFormation itemFormation;
    private ItemKit itemKit;
    private int changesRemaning;
    private int changesMax = 3;

    private GameObject selectedGo;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        currentBattleType = BattleType.Mini;
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
        selectedGo = null;
        InitializeUI();
        PopulateUI();
    }

    public override void Hide()
    {
        base.SetInteractable(false);
        base.Hide();

        currentTeam = null;
        selectedGo = null;
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

    private void InitializeUI() 
    {
        panelActive.SetActive(isEditMode);
        panelTeamButtons.SetActive(isEditMode);
        panelChanges.SetActive(isBattleMode);

        changesRemaning = changesMax;
    }

    private void PopulateUI()
    {
        if (currentTeam == null) return;
        UpdateSetActiveButtonState();
        formationLayoutUI.Initialize(currentTeam, currentBattleType);
    }

    #endregion

    #region Input

    // TODO E sumary, q replace, m change battle type

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

    public void OnButtonTeamActionsClicked() 
    { 
        UIEvents.RaiseTeamActionsOpened(currentTeam); 
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamLoadoutSelected += HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUISelectedDefault += HandleFormationCharacterSlotUISelectedDefault;
        UIEvents.OnFormationCharacterSlotUIClicked += HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISwaped += HandeFormationCharacterSlotUISwaped;
        UIEvents.OnFormationCharacterSlotUIReplaced += HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIReplaceRequested += HandleFormationCharacterSlotUIReplaceRequested;
        UIEvents.OnTeamButtonSelected += HandleTeamButtonSelected;
        UIEvents.OnCharacterSelected += HandleCharacterSelected;
        UIEvents.OnItemSelected += HandleItemSelected;
        UIEvents.OnBattleTypeChangeRequested += HandleBattleTypeChangeRequested;
        UIEvents.OnTeamEmblemChanged += HandleTeamEmblemChanged;
        UIEvents.OnTeamNameChanged += HandleTeamNameChanged;
        UIEvents.OnTeamActionsClosed += HandleTeamActionsClosed;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamLoadoutSelected -= HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUISelectedDefault -= HandleFormationCharacterSlotUISelectedDefault;
        UIEvents.OnFormationCharacterSlotUIClicked -= HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISwaped -= HandeFormationCharacterSlotUISwaped;
        UIEvents.OnFormationCharacterSlotUIReplaced -= HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIReplaceRequested -= HandleFormationCharacterSlotUIReplaceRequested;
        UIEvents.OnTeamButtonSelected -= HandleTeamButtonSelected;
        UIEvents.OnCharacterSelected -= HandleCharacterSelected;
        UIEvents.OnItemSelected -= HandleItemSelected;
        UIEvents.OnBattleTypeChangeRequested -= HandleBattleTypeChangeRequested;
        UIEvents.OnTeamEmblemChanged -= HandleTeamEmblemChanged;
        UIEvents.OnTeamNameChanged -= HandleTeamNameChanged;
        UIEvents.OnTeamActionsClosed -= HandleTeamActionsClosed;
    }

    private void HandleLoadoutSelected(Team team) 
    {
        currentTeam = team;
        menuManager.OpenMenu(this);
    }

    private void HandleFormationCharacterSlotUISelectedDefault(FormationCharacterSlotUI slot) 
    {
        if (slot == null || slot.gameObject == null) return;

        selectedGo = slot.gameObject;
        base.SetDefaultSelectable(slot.GetComponent<Button>());
    }

    private void HandleFormationCharacterSlotUIClicked(FormationCharacterSlotUI slot) 
    {
        if (slot == null || slot.gameObject == null) return;

        selectedGo = slot.gameObject;
        base.SetLastSelected(selectedGo);

        if (!isEditMode) return;
        currentSlot = slot;
        UIEvents.RaiseCharacterActionsOpened();
    }

    private void HandeFormationCharacterSlotUISwaped(FormationCharacterSlotUI a, FormationCharacterSlotUI b) 
    {
        if(isBattleMode && changesRemaning <= 0) 
        {
            return;
        } 

        Character temp = a.GetCharacter();
        a.SetCharacter(b.GetCharacter());
        b.SetCharacter(temp);

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

        if(isBattleMode) changesRemaning--;
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
        selectedGo = gameObject;
        base.SetLastSelected(selectedGo);
    }

    private void HandleCharacterSelected(Character character) 
    {
        selectedCharacter = character;
        if(isOpen && isEditMode)
            UIEvents.RaiseFormationCharacterSlotUIReplaceRequested();
    }

    private void HandleItemSelected(Item item) 
    {
        if (item.Category == ItemCategory.Formation) 
        {
            itemFormation = item as ItemFormation;
            currentTeam.SetFormation(FormationManager.Instance.GetFormation(itemFormation.FormationId), currentBattleType);
            formationLayoutUI.SetFormation(currentTeam.GetFormation(currentBattleType));
        }

        if (item.Category == ItemCategory.Kit) 
        {
            itemKit = item as ItemKit;
            currentTeam.SetKit(KitManager.Instance.GetKit(itemKit.KitId));
            formationLayoutUI.SetKit(currentTeam.Kit);
            // TeamEvents.RaiseKitChanged(currentTeam, KitManager.Instance.GetKit(itemKit.KitId));
        }
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    public void HandleBattleTypeChangeRequested()
    {
        if(!isEditMode) return;

        BattleType oldType = currentBattleType;

        currentBattleType = currentBattleType == BattleType.Full
            ? BattleType.Mini
            : BattleType.Full;

        formationLayoutUI.Initialize(currentTeam, currentBattleType);
        UIEvents.RaiseBattleTypeChanged(currentBattleType, oldType);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    public void HandleTeamEmblemChanged(string emblemId)
    {
        if(!isEditMode) return;
        currentTeam.UpdateAppeariance(emblemId);
        formationLayoutUI.Initialize(currentTeam, currentBattleType);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    public void HandleTeamNameChanged(string newName)
    {
        if(!isEditMode) return;
        currentTeam.SetCustomName(newName);
        formationLayoutUI.Initialize(currentTeam, currentBattleType);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    public void HandleTeamActionsClosed()
    {
        if (selectedGo == null)
        {
            if (formationLayoutUI != null)
            {
                SetDefaultFocus();
            }
            return;
        }

        Button btn = selectedGo.GetComponent<Button>();
        if (btn != null)
            base.SetDefaultSelectable(btn);
        else
            SetDefaultFocus();
    }



    #endregion
}
