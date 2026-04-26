using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

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
    private SubstitutionManager substitutionManager;
    private InputManager inputManager;
    private FormationManager formationDatabase;
    private KitManager kitDatabase;

    private Team currentTeam;
    private FormationCharacterSlotUI currentSlot;
    private BattleType currentBattleType;
    private BattleType defaultBattleType = BattleType.Mini;
    private Character selectedCharacter;
    private GameObject selectedGo;

    private ItemFormation cachedItemFormation;
    private ItemKit cachedItemKit;
    private Formation cachedFormation;
    private Kit cachedKit;

    private bool isClosing = false;
    private bool hasSwapped = false;

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

        currentBattleType = defaultBattleType;
        menuManager = MenuManager.Instance;
        teamManager = TeamManager.Instance;
        substitutionManager = SubstitutionManager.Instance;
        inputManager = InputManager.Instance;
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
        hasSwapped = false;

        if(isBattleMode && inputManager.IsAndroid) InputEvents.RaiseScreenControlsHideRequested();

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

        if(isBattleMode && inputManager.IsAndroid) InputEvents.RaiseScreenControlsShowRequested();

        currentTeam = null;
        selectedGo = null;
        pickedSlot = null;
    }

    public void Close()
    {
        if (!isTop) return;
        UIEvents.RaiseBackFromTeamRequested(currentTeam, hasSwapped);
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
        panelChanges.SetActive(isBattleMode && currentBattleType == BattleType.Full);

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
        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode);
    }

    #endregion

    #region Input

    // click on character -> character actions
    // TODO E sumary, r replace, q move
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

    private void UpdateChangesText(int currentValue, int maxValue)
    {
        textChanges.text = $"{currentValue}/{maxValue}";
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
        UIEvents.OnMenuTeamBattleRequested += HandleMenuTeamBattleRequested;
        UIEvents.OnSubstitutionChangesUpdated += HandleSubstitutionChangesUpdated;
        BattleEvents.OnBattleStart += HandleBattleStart;
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
        UIEvents.OnMenuTeamBattleRequested -= HandleMenuTeamBattleRequested;
        UIEvents.OnSubstitutionChangesUpdated -= HandleSubstitutionChangesUpdated;
        BattleEvents.OnBattleStart -= HandleBattleStart;
    }

    private void HandleLoadoutSelected(Team team) 
    {
        currentTeam = team;
        menuManager.OpenMenu(this);
    }

    private void HandleMenuTeamBattleRequested(Team team) 
    {
        if (isOpen) return;
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
        if (!isTop) return;
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
        // check substitution
        bool isValidSwap = isEditMode || substitutionManager.ValidateSwap(currentTeam.TeamSide, a , b);
        if (!isValidSwap) return;
        hasSwapped = true;

        string guidA = a.GetCharacter().CharacterGuid;
        string guidB = b.GetCharacter().CharacterGuid;

        // swap entities in battle
        if (isBattleMode) 
        {
            teamManager.SwapCharactersInBattle(
                currentTeam,
                currentBattleType,
                a.SlotIndex, a.FormationCoord, guidA,
                b.SlotIndex, b.FormationCoord, guidB
            );
        }

        if (isEditMode) 
        {
            a.GetCharacter().ApplyKit(
                currentTeam.Kit,
                currentTeam.Variant,
                b.FormationCoord.Position
            );

            b.GetCharacter().ApplyKit(
                currentTeam.Kit,
                currentTeam.Variant,
                a.FormationCoord.Position
            );

            teamManager.SetCharacterInLoadout(
                currentTeam,
                currentBattleType,
                a.SlotIndex,
                guidB
            );

            teamManager.SetCharacterInLoadout(
                currentTeam,
                currentBattleType,
                b.SlotIndex,
                guidA
            );
        }

        // Update visual slot
        Character temp = a.GetCharacter();
        a.SetCharacter(b.GetCharacter());
        b.SetCharacter(temp);
    }

    private void HandleFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character)
    {
        teamManager.SetCharacterInLoadout(
            currentTeam,
            currentBattleType,
            currentSlot.SlotIndex,
            character.CharacterGuid
        );
    }

    private void HandleFormationCharacterSlotUIReplaceRequested() 
    {
        if (selectedCharacter == null) return;
        teamManager.SetCharacterInLoadout(
            currentTeam,
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

        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode);
        UIEvents.RaiseBattleTypeChanged(currentBattleType, oldType);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void HandleTeamEmblemChanged(string emblemId)
    {
        if(!isEditMode) return;
        currentTeam.UpdateAppeariance(emblemId);
        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void HandleTeamNameChanged(string newName)
    {
        if(!isEditMode) return;
        currentTeam.SetCustomName(newName);
        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode);
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

    private void HandleSubstitutionChangesUpdated(int currentValue, int maxValue) 
    {
        UpdateChangesText(currentValue, maxValue);
    }

    private void HandleBattleStart(BattleType battleType) 
    {
        currentBattleType = battleType;
    }

    #endregion
}
