using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Input;

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
    [SerializeField] private Button buttonCharacterSummarySide;

    [SerializeField] private TMP_Text textChanges;

    [SerializeField] private FormationLayoutUI formationLayoutUI;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;
    private TeamManager teamManager;
    private SubstitutionManager substitutionManager;
    private InputManager inputManager;
    private FormationManager formationDatabase;
    private KitManager kitDatabase;
    private AudioManager audioManager;

    private Team currentTeam;
    private FormationCharacterSlotUI currentSlot;
    private FormationCharacterSlotUI selectedSlot;
    private BattleType currentBattleType;
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
    private bool isReplacing = false;
    private bool isPlaySfxSelectEnabled = false;

    #endregion

    #region Lifecycle

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
        teamManager = TeamManager.Instance;
        substitutionManager = SubstitutionManager.Instance;
        inputManager = InputManager.Instance;
        formationDatabase = FormationManager.Instance;
        kitDatabase = KitManager.Instance;
        audioManager = AudioManager.Instance;

        currentBattleType = teamManager.DefaultBattleType;
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
        selectedSlot = null;
    }

    public void Close()
    {
        if (!isTop) return;
        pickedSlot = null;
        selectedSlot = null;
        isReplacing = false;
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

        if (interactable) 
            SubscribeInput();
        else
            UnsubscribeInput();

        isPlaySfxSelectEnabled = interactable;
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
        buttonCharacterSummarySide.interactable = isEditMode;

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

    private void SubscribeInput()
    {
        inputManager.SubscribeDown(CustomAction.Navigation_Back, HandleShortcutBack);
        inputManager.SubscribeDown(CustomAction.Navigation_ShortcutTeamBattleType, HandleShortcutTeamBattleType);
        inputManager.SubscribeDown(CustomAction.Navigation_ShortcutTeamActive, HandleShortcutTeamActive);
        inputManager.SubscribeDown(CustomAction.Navigation_ShortcutTeamActions, HandleShortcutTeamActions);
        inputManager.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterSummary, HandleShortcutTeamCharacterSummary);
        inputManager.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterMove, HandleShortcutCharacterMove);
        inputManager.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterReplace, HandleShortcutTeamCharacterReplace);
        inputManager.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext, HandleShortcutTeamCharacterNext);
    }

    private void UnsubscribeInput()
    {
        inputManager.UnsubscribeDown(CustomAction.Navigation_Back, HandleShortcutBack);
        inputManager.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamBattleType, HandleShortcutTeamBattleType);
        inputManager.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamActive, HandleShortcutTeamActive);
        inputManager.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamActions, HandleShortcutTeamActions);
        inputManager.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterSummary, HandleShortcutTeamCharacterSummary);
        inputManager.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterMove, HandleShortcutCharacterMove);
        inputManager.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterReplace, HandleShortcutTeamCharacterReplace);
        inputManager.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext, HandleShortcutTeamCharacterNext);
    }

    private void HandleShortcutBack()
    {
        if (isReplacing) return;

        if (isSwapping)
        {
            UIEvents.RaiseFormationCharacterSlotUIMoveCanceled(pickedSlot);
            return;
        }

        audioManager.PlaySfx("sfx-menu_back");
        Close();
    }

    private void HandleShortcutTeamBattleType()
    {
        if (isSwapping || isReplacing) return;
        if (isBattleMode) return;
        audioManager.PlaySfx("sfx-menu_tap");
        UIEvents.RaiseBattleTypeChangeRequested();
    }

    private void HandleShortcutTeamActive()
    {
        if (isSwapping || isReplacing) return;
        if (isBattleMode) return;
        OnButtonSetActiveClicked();
    }

    private void HandleShortcutTeamActions()
    {
        if (isSwapping || isReplacing) return;
        if (isBattleMode) return;
        audioManager.PlaySfx("sfx-menu_tap");
        UIEvents.RaiseTeamActionsOpened(currentTeam, currentBattleType);
    }

    private void HandleShortcutTeamCharacterSummary()
    {
        if (isSwapping || isReplacing) return;
        if (isBattleMode) return;
        if (selectedSlot == null || selectedSlot.GetCharacter() == null) return;
        audioManager.PlaySfx("sfx-menu_tap");
        UIEvents.RaiseCharacterDetailOpenRequested(selectedSlot.GetCharacter());
    }

    private void HandleShortcutCharacterMove()
    {
        if (isSwapping || isReplacing) return;
        UIEvents.RaiseFormationCharacterSlotUIMoveRequested(null);
    }

    private void HandleShortcutTeamCharacterReplace()
    {
        if (isSwapping || isReplacing) return;
        if (isBattleMode) return;
        audioManager.PlaySfx("sfx-menu_tap");
        UIEvents.RaiseFormationCharacterSlotUIReplaceRequested();
        UIEvents.RaiseCharacterSelectorOpenRequested(
            CharacterSelectorModePopulate.GetFromStorage,
            CharacterSelectorModeClick.SelectCharacter,
            null,
            default,
            true);
    }

    private void HandleShortcutTeamCharacterNext()
    {
        if (isSwapping || isReplacing) return;
        UIEvents.RaiseCharacterDetailSideNextPageRequested();
    }

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
        audioManager.PlaySfx("sfx-menu_tap");
    }

    public void OnButtonCloseClicked()
    {
        audioManager.PlaySfx("sfx-menu_back");
        Close();
    }

    public void OnButtonSelected(GameObject gameObject) 
    {
        UIEvents.RaiseTeamButtonSelected(gameObject);
    }

    public void OnButtonTeamActionsClicked() 
    {
        audioManager.PlaySfx("sfx-menu_tap"); 
        UIEvents.RaiseTeamActionsOpened(currentTeam, currentBattleType); 
    }

    public void OnButtonDeleteClicked() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        UIEvents.RaiseTeamPanelDeleteOpened(currentTeam);
    }

    public void OnButtonSelectedSfx() { 
        if (isPlaySfxSelectEnabled)
            audioManager.PlaySfx("sfx-menu_selected");
    }

    public void OnButtonPointerEnter(Selectable selectable) 
    {
        base.SetDefaultSelectable(selectable);
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamLoadoutSelected += HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUISelectedDefault += HandleFormationCharacterSlotUISelectedDefault;
        UIEvents.OnFormationCharacterSlotUIClicked += HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISelected += HandleFormationCharacterSlotUISelected;
        UIEvents.OnFormationCharacterSlotUIHighlighted += HandleFormationCharacterSlotUIHighlighted;
        UIEvents.OnFormationCharacterSlotUISwapped += HandleFormationCharacterSlotUISwapped;
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
        UIEvents.OnTeamCharacterActionsClosed += HandleTeamCharacterActionsClosed;
        UIEvents.OnBackFromCharacterSelectorRequested += HandleBackFromCharacterSelectorRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamLoadoutSelected -= HandleLoadoutSelected;
        UIEvents.OnFormationCharacterSlotUISelectedDefault -= HandleFormationCharacterSlotUISelectedDefault;
        UIEvents.OnFormationCharacterSlotUIClicked -= HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISelected -= HandleFormationCharacterSlotUISelected;
        UIEvents.OnFormationCharacterSlotUIHighlighted -= HandleFormationCharacterSlotUIHighlighted;
        UIEvents.OnFormationCharacterSlotUISwapped -= HandleFormationCharacterSlotUISwapped;
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
        UIEvents.OnTeamCharacterActionsClosed -= HandleTeamCharacterActionsClosed;
        UIEvents.OnBackFromCharacterSelectorRequested -= HandleBackFromCharacterSelectorRequested;
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
        base.SetDefaultSelectable(slot.Button);
        UIEvents.RaiseFormationCharacterSlotUISelected(slot);
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
                UIEvents.RaiseFormationCharacterSlotUISwapped(pickedSlot, slot);
            isPlaySfxSelectEnabled = false;
            base.SetDefaultSelectable(slot.Button);
            UIEvents.RaiseFormationCharacterSlotUIMoveEnded(pickedSlot);
            pickedSlot = null;
            //audioManager.PlaySfx("sfx-menu_tap");
            isPlaySfxSelectEnabled = true;
            return;
        }

        audioManager.PlaySfx("sfx-menu_tap");
        currentSlot = slot;
        selectedSlot = slot;
        UIEvents.RaiseTeamCharacterActionsOpenRequested(slot.GetCharacter());
    }

    private void HandleFormationCharacterSlotUIMoveCanceled(FormationCharacterSlotUI slot) 
    {
        pickedSlot = null;
        audioManager.PlaySfx("sfx-menu_cancel");
        UIEvents.RaiseFormationCharacterSlotUIMoveEnded(slot);
    }

    private void HandleFormationCharacterSlotUIMoveRequested(FormationCharacterSlotUI slot)
    {
        base.SetDefaultSelectable(selectedSlot.Button);
        pickedSlot = selectedSlot;
        audioManager.PlaySfx("sfx-menu_change");
        UIEvents.RaiseFormationCharacterSlotUIMoveStarted(selectedSlot);
    }

    private void HandleFormationCharacterSlotUISwapped(FormationCharacterSlotUI a, FormationCharacterSlotUI b) 
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

        audioManager.PlaySfx("sfx-menu_tap");
        isPlaySfxSelectEnabled = false;
        base.SetDefaultSelectable(a.Button);
        isPlaySfxSelectEnabled = true;
    }

    private void HandleFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character)
    {
        if (!isOpen || character == null || slot == null) return;

        string newGuid = character.CharacterGuid;

        // If the character is already in the team, treat it as a swap
        if (currentTeam.ContainsCharacterGuid(newGuid, currentBattleType))
        {
            FormationCharacterSlotUI otherSlot = formationLayoutUI.FindSlotByCharacterGuid(newGuid);
            if (otherSlot != null && otherSlot != slot)
            {
                UIEvents.RaiseFormationCharacterSlotUISwapped(slot, otherSlot);
            }
        }
        else
        {
            //hasSwapped = true;

            // Store the old character reference before replacing
            Character oldCharacter = slot.GetCharacter();

            // Apply kit to the new character for this position
            if (isEditMode)
            {
                character.ApplyKit(
                    currentTeam.Kit,
                    currentTeam.Variant,
                    slot.FormationCoord.Position
                );
            }

            // Update the loadout data
            teamManager.SetCharacterInLoadout(
                currentTeam,
                currentBattleType,
                slot.SlotIndex,
                newGuid
            );

            // Update the visual slot
            slot.SetCharacter(character);
        }

        isPlaySfxSelectEnabled = false;
        base.SetDefaultSelectable(slot.Button);
        isPlaySfxSelectEnabled = true;
    }

    private void HandleFormationCharacterSlotUIReplaceRequested() 
    {
        isReplacing = true;
    }

    private void HandleTeamButtonSelected(GameObject gameObject) 
    {
        selectedGo = gameObject;
        base.SetLastSelected(selectedGo);
    }

    private void HandleCharacterSelected(Character character) 
    {
        selectedCharacter = character;
        if(isReplacing) 
        {
            audioManager.PlaySfx("sfx-menu_tap");
            UIEvents.RaiseFormationCharacterSlotUIReplaced(selectedSlot, selectedCharacter);
        }

        isReplacing = false;
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

        isPlaySfxSelectEnabled = false;
        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode);
        teamManager.SetDefaultBattleType(currentBattleType);
        UIEvents.RaiseBattleTypeChanged(currentBattleType, oldType);
        UIEvents.RaiseBackFromTeamActionsRequested();
        isPlaySfxSelectEnabled = true;
    }

    private void HandleTeamEmblemChanged(string emblemId)
    {
        if(!isEditMode) return;
        currentTeam.UpdateAppearance(emblemId);
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

    private void HandleTeamCharacterActionsClosed() 
    {
        if (currentSlot != null)
            base.SetDefaultSelectable(currentSlot.Button);
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

    private void HandleBackFromCharacterSelectorRequested() 
    {
        isReplacing = false;
    }

    private void HandleFormationCharacterSlotUISelected(FormationCharacterSlotUI slot) 
    {
        if (!isTop) return;
        selectedSlot = slot;

        if (isPlaySfxSelectEnabled) 
            audioManager.PlaySfx("sfx-menu_selected");
    }

    private void HandleFormationCharacterSlotUIHighlighted(FormationCharacterSlotUI slot) 
    {
        if (!isTop) return;
        base.SetDefaultSelectable(slot.Button);
    }

    #endregion
}
