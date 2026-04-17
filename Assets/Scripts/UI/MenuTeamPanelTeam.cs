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
    [SerializeField] private GameObject buttonDelete;
    [SerializeField] private Button buttonOptions;
    [SerializeField] private Button buttonCharacterReplace;
    [SerializeField] private Button buttonCharacterSummary;
    // [SerializeField] private GameObject buttonCharacterSummarySide;

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
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;
    private TeamManager teamManager;
    private FormationManager formationDatabase;
    private KitManager kitDatabase;

    private Team currentTeam;
    private FormationCharacterSlotUI currentSlot;
    private BattleType currentBattleType;
    private Character selectedCharacter;
    private GameObject selectedGo;

    private ItemFormation cachedItemFormation;
    private ItemKit cachedItemKit;
    private Formation cachedFormation;
    private Kit cachedKit;

    private bool isClosing = false;

    private FormationCharacterSlotUI pickedSlot;
    private bool isSwapping => pickedSlot != null;

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
        formationDatabase = FormationManager.Instance;
        kitDatabase = KitManager.Instance;
    }

    private void OnDestroy()
    {

    }

    #endregion

   #region Menu Overrides

    public override void Show()
    {
        isClosing = false;

        base.Show();
        base.SetInteractable(true);

        selectedCharacter = null;
        selectedGo = null;
        InitializeUI();
        PopulateUI();
    }

    public override void Hide()
    {
        isClosing = false;

        base.SetInteractable(false);
        base.Hide();

        currentTeam = null;
        selectedGo = null;
        pickedSlot = null;
    }

    public void Close()
    {
        if (!isTop) return;
        if(!isEditMode) return;
        UIEvents.RaiseBackFromTeamRequested();
        menuManager.CloseMenu();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);

        if (interactable && isClosing)
        {
            isClosing = false;
            Close();
        }
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
        panelChanges.SetActive(isBattleMode);

        buttonOptions.interactable = isEditMode;
        buttonCharacterReplace.interactable = isEditMode;
        buttonCharacterSummary.interactable = isEditMode;
        // buttonCharacterSummarySide.interactable = isEditMode;

        buttonDelete.SetActive(isEditMode && teamManager.ActiveLoadoutGuid != currentTeam.TeamGuid);
    }

    private void PopulateUI()
    {
        if (currentTeam == null) return;
        UpdateSetActiveButtonState();
        formationLayoutUI.Initialize(currentTeam, currentBattleType);
    }

    #endregion

    #region Input

    // click on character -> character actions
    // TODO E sumary, r replace, q swap
    // TODO T team actions, shift change battle type, f set active

    #endregion

    #region Helper

    private bool isEditMode => mode == MenuTeamMode.Edit;
    private bool isBattleMode => mode == MenuTeamMode.Battle;

    private void UpdateSetActiveButtonState() 
    {
        activeImage.enabled = teamManager.ActiveLoadoutGuid == currentTeam.TeamGuid;
        buttonDelete.SetActive(isEditMode && teamManager.ActiveLoadoutGuid != currentTeam.TeamGuid);
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
        UIEvents.RaiseTeamActionsOpened(currentTeam, currentBattleType); 
    }

    public void OnButtonDeleteClicked() 
    {
        UIEvents.RaiseTeamPanelDeleteOpened(currentTeam);
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
        UIEvents.OnSelectorItemSideListItemSelected += HandleSelectorItemSideListItemSelected;
        UIEvents.OnSelectorItemSideListItemHighlighted += HandleSelectorItemSideListItemHighlighted;
        UIEvents.OnBackFromSelectorItemSideRequested += HandleBackFromSelectorItemSideRequested;
        UIEvents.OnBattleTypeChangeRequested += HandleBattleTypeChangeRequested;
        UIEvents.OnTeamEmblemChanged += HandleTeamEmblemChanged;
        UIEvents.OnTeamNameChanged += HandleTeamNameChanged;
        UIEvents.OnTeamActionsClosed += HandleTeamActionsClosed;
        TeamEvents.OnLoadoutDeleted += HandleLoadoutDeleted;
        UIEvents.OnFormationCharacterSlotUIMoveRequested += HandleFormationCharacterSlotUIMoveRequested;
        UIEvents.OnFormationCharacterSlotUIMoveCanceled += HandleFormationCharacterSlotUIMoveCanceled;
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
        UIEvents.OnSelectorItemSideListItemSelected -= HandleSelectorItemSideListItemSelected;
        UIEvents.OnSelectorItemSideListItemHighlighted -= HandleSelectorItemSideListItemHighlighted;
        UIEvents.OnBackFromSelectorItemSideRequested -= HandleBackFromSelectorItemSideRequested;
        UIEvents.OnBattleTypeChangeRequested -= HandleBattleTypeChangeRequested;
        UIEvents.OnTeamEmblemChanged -= HandleTeamEmblemChanged;
        UIEvents.OnTeamNameChanged -= HandleTeamNameChanged;
        UIEvents.OnTeamActionsClosed -= HandleTeamActionsClosed;
        TeamEvents.OnLoadoutDeleted -= HandleLoadoutDeleted;
        UIEvents.OnFormationCharacterSlotUIMoveRequested -= HandleFormationCharacterSlotUIMoveRequested;
        UIEvents.OnFormationCharacterSlotUIMoveCanceled -= HandleFormationCharacterSlotUIMoveCanceled;
    }

    private void HandleLoadoutSelected(Team team) 
    {
        currentTeam = team;
        menuManager.OpenMenu(this);
    }

    private void HandleFormationCharacterSlotUISelectedDefault(FormationCharacterSlotUI slot) 
    {
        if (!isTop) return;
        if (slot == null || slot.gameObject == null) return;

        selectedGo = slot.gameObject;
        base.SetDefaultSelectable(slot.GetComponent<Button>());
    }

    private void HandleFormationCharacterSlotUIClicked(FormationCharacterSlotUI slot) 
    {
        if (slot == null || slot.gameObject == null) return;

        selectedGo = slot.gameObject;
        base.SetLastSelected(selectedGo);

        if (isSwapping) 
        {
            if (pickedSlot != slot) 
                UIEvents.RaiseFormationCharacterSlotUISwaped(pickedSlot, slot);
            base.SetDefaultSelectable(slot.GetComponent<Button>());
            UIEvents.RaiseFormationCharacterSlotUIMoveEnded(pickedSlot);
            pickedSlot = null;
            return;
        }

        if (!isEditMode) return;
        currentSlot = slot;
        UIEvents.RaiseCharacterActionsOpened();
    }


    private void HandleFormationCharacterSlotUIMoveCanceled(FormationCharacterSlotUI slot) 
    {
        pickedSlot = null;
        UIEvents.RaiseFormationCharacterSlotUIMoveEnded(slot);
    }


    private void HandleFormationCharacterSlotUIMoveRequested(FormationCharacterSlotUI slot)
    {
        base.SetDefaultSelectable(currentSlot.GetComponent<Button>());
        pickedSlot = currentSlot;
        UIEvents.RaiseFormationCharacterSlotUIMoveStarted(currentSlot);
    }










    private void HandeFormationCharacterSlotUISwaped(FormationCharacterSlotUI a, FormationCharacterSlotUI b) 
    {
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
        teamManager.SetCharacterInLoadout(
            currentTeam.TeamGuid,
            currentBattleType,
            currentSlot.SlotIndex,
            selectedCharacter.CharacterGuid
        );
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
        if(!isOpen || !isEditMode) return;

        if (item.Category == ItemCategory.Formation) 
        {
            cachedItemFormation = item as ItemFormation;
            cachedFormation = formationDatabase.GetFormation(cachedItemFormation.FormationId);  
            currentTeam.SetFormation(cachedFormation, currentBattleType);
            formationLayoutUI.SetFormation(cachedFormation);
        }

        if (item.Category == ItemCategory.Kit) 
        {
            cachedItemKit = item as ItemKit;
            cachedKit = kitDatabase.GetKit(cachedItemKit.KitId);
            currentTeam.SetKit(cachedKit);
            formationLayoutUI.SetKit(cachedKit);
            // TeamEvents.RaiseKitChanged(currentTeam, KitManager.Instance.GetKit(itemKit.KitId));
        }
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void PreviewHandleSelectorItemSideListItem(SelectorItemSideListItem listItem) 
    {
        if(!isOpen || !isEditMode) return;

        if (listItem.ItemStorageSlot.Item.Category == ItemCategory.Formation) 
        {
            cachedItemFormation = listItem.ItemStorageSlot.Item as ItemFormation;
            cachedFormation = formationDatabase.GetFormation(cachedItemFormation.FormationId);
            formationLayoutUI.SetFormation(cachedFormation, false);
        }

        if (listItem.ItemStorageSlot.Item.Category == ItemCategory.Kit) 
        {
            cachedItemKit = listItem.ItemStorageSlot.Item as ItemKit;
            cachedKit = kitDatabase.GetKit(cachedItemKit.KitId);
            formationLayoutUI.SetKit(cachedKit, false);
        }
    }

    private void HandleBackFromSelectorItemSideRequested(ItemCategory category) 
    {
        if(!isOpen || !isEditMode) return;

        if (category == ItemCategory.Formation) 
            formationLayoutUI.SetFormation(currentTeam.GetFormation(currentBattleType));

        if (category == ItemCategory.Kit) 
            formationLayoutUI.SetKit(currentTeam.Kit);
    }

    private void HandleSelectorItemSideListItemHighlighted(SelectorItemSideListItem listItem) 
    {
        PreviewHandleSelectorItemSideListItem(listItem);
    }

    private void HandleSelectorItemSideListItemSelected(SelectorItemSideListItem listItem) 
    {
        PreviewHandleSelectorItemSideListItem(listItem);
    }

    private void HandleBattleTypeChangeRequested()
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

    private void HandleTeamEmblemChanged(string emblemId)
    {
        if(!isEditMode) return;
        currentTeam.UpdateAppeariance(emblemId);
        formationLayoutUI.Initialize(currentTeam, currentBattleType);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void HandleTeamNameChanged(string newName)
    {
        if(!isEditMode) return;
        currentTeam.SetCustomName(newName);
        formationLayoutUI.Initialize(currentTeam, currentBattleType);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void HandleTeamActionsClosed()
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

    private void HandleLoadoutDeleted(Team team)
    { 
        isClosing = true;
    }

    #endregion
}
