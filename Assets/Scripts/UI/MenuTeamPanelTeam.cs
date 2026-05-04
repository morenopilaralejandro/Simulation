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

    // Cached managers
    private TeamManager           teamManager;
    private SubstitutionManager   substitutionManager;
    private InputManager          inputManager;
    private FormationManager      formationDatabase;
    private KitManager            kitDatabase;
    private AudioManager          audioManager;

    // Runtime state
    private Team                       currentTeam;
    private BattleType                 currentBattleType;
    private FormationCharacterSlotUI   currentSlot;
    private FormationCharacterSlotUI   selectedSlot;
    private FormationCharacterSlotUI   pickedSlot;
    private bool                       hasSwapped;

    // Cached selections
    private ItemFormation cachedItemFormation;
    private ItemKit       cachedItemKit;
    private Formation     cachedFormation;
    private Kit           cachedKit;

    private MenuStateMachine<MenuTeamState> stateMachine;

    private bool isEditMode   => mode == MenuTeamMode.Edit;
    private bool isBattleMode => mode == MenuTeamMode.Battle;

    #endregion

    #region Lifecycle

    private void Start()
    {
        teamManager         = TeamManager.Instance;
        substitutionManager = SubstitutionManager.Instance;
        inputManager        = InputManager.Instance;
        formationDatabase   = FormationManager.Instance;
        kitDatabase         = KitManager.Instance;
        audioManager        = AudioManager.Instance;

        currentBattleType   = teamManager.DefaultBattleType;

        BuildStateMachine();
    }

    private void BuildStateMachine()
    {
        stateMachine = new MenuStateMachine<MenuTeamState>(MenuTeamState.Idle)
            .OnEnter(MenuTeamState.Swapping, () =>
            {
                pickedSlot = selectedSlot;
                audioManager.PlaySfx("sfx-menu_change");
                UIEvents.RaiseFormationCharacterSlotUIMoveStarted(selectedSlot);
            })
            .OnExit(MenuTeamState.Swapping, () =>
            {
                UIEvents.RaiseFormationCharacterSlotUIMoveEnded(pickedSlot);
                pickedSlot = null;
            })
            .OnEnter(MenuTeamState.Replacing, () =>
            {
                UIEvents.RaiseFormationCharacterSlotUIReplaceRequested();
                UIEvents.RaiseCharacterSelectorOpenRequested(
                    source:        new SelectorCharacterSourceFromStorage(),
                    action:        new SelectorCharacterAction(),
                    filter:        null,           // or new ExcludeGuidsFilter(currentTeam.GetCharacterGuids(currentBattleType))
                    closeOnSelect: true);
            });
    }

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        hasSwapped = false;

        if (isBattleMode && inputManager.IsAndroid)
            InputEvents.RaiseScreenControlsHideRequested();

        base.Show();

        InitializeUI();
        PopulateUI();
    }

    public override void Hide()
    {
        if (isBattleMode && inputManager.IsAndroid)
            InputEvents.RaiseScreenControlsShowRequested();

        currentTeam  = null;
        selectedSlot = null;
        pickedSlot   = null;
        currentSlot  = null;

        base.Hide();
    }

    protected override void OnGainedInput()
    {
        var im = inputManager;
        im.SubscribeDown(CustomAction.Navigation_Back,                            HandleShortcutBack);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamBattleType,          HandleShortcutTeamBattleType);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamActive,              HandleShortcutTeamActive);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamActions,             HandleShortcutTeamActions);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterSummary,    HandleShortcutTeamCharacterSummary);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterMove,       HandleShortcutCharacterMove);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterReplace,    HandleShortcutTeamCharacterReplace);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext,       HandleShortcutTeamCharacterNext);
    }

    protected override void OnLostInput()
    {
        var im = inputManager;
        im.UnsubscribeDown(CustomAction.Navigation_Back,                          HandleShortcutBack);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamBattleType,        HandleShortcutTeamBattleType);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamActive,            HandleShortcutTeamActive);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamActions,           HandleShortcutTeamActions);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterSummary,  HandleShortcutTeamCharacterSummary);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterMove,     HandleShortcutCharacterMove);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterReplace,  HandleShortcutTeamCharacterReplace);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext,     HandleShortcutTeamCharacterNext);
    }

    public void Refresh() => PopulateUI();

    #endregion

    #region Populate

    private void InitializeUI()
    {
        panelActive .SetActive(isEditMode);
        panelChanges.SetActive(isBattleMode && currentBattleType == BattleType.Full);

        buttonOptions             .interactable = isEditMode;
        buttonCharacterReplace    .interactable = isEditMode;
        buttonCharacterSummary    .interactable = isEditMode;
        buttonCharacterSummarySide.interactable = isEditMode;

        buttonDelete.SetActive(isEditMode && teamManager.ActiveLoadoutGuid != currentTeam.TeamGuid);
    }

    private void PopulateUI()
    {
        if (currentTeam == null) return;
        UpdateSetActiveButtonState();
        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode);
    }

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

    #region Input Shortcuts

    private void HandleShortcutBack()
    {
        if (stateMachine.Is(MenuTeamState.Replacing)) return;

        if (stateMachine.Is(MenuTeamState.Swapping))
        {
            stateMachine.Set(MenuTeamState.Idle);
            UIEvents.RaiseFormationCharacterSlotUIMoveCanceled(pickedSlot);
            return;
        }

        UIEvents.RaiseBackFromTeamRequested(currentTeam, hasSwapped);
        RequestClose();
    }

    private void HandleShortcutTeamBattleType()
    {
        if (!stateMachine.Is(MenuTeamState.Idle) || isBattleMode) return;
        UIEvents.RaiseBattleTypeChangeRequested();
    }

    private void HandleShortcutTeamActive()
    {
        if (!stateMachine.Is(MenuTeamState.Idle) || isBattleMode) return;
        OnButtonSetActiveClicked();
    }

    private void HandleShortcutTeamActions()
    {
        if (!stateMachine.Is(MenuTeamState.Idle) || isBattleMode) return;
        UIEvents.RaiseTeamActionsOpened(currentTeam, currentBattleType);
    }

    private void HandleShortcutTeamCharacterSummary()
    {
        if (!stateMachine.Is(MenuTeamState.Idle) || isBattleMode) return;
        if (selectedSlot == null || selectedSlot.GetCharacter() == null) return;
        UIEvents.RaiseCharacterDetailOpenRequested(selectedSlot.GetCharacter());
    }

    private void HandleShortcutCharacterMove()
    {
        if (!stateMachine.Is(MenuTeamState.Idle)) return;
        if (selectedSlot == null) return;
        stateMachine.Set(MenuTeamState.Swapping);
    }

    private void HandleShortcutTeamCharacterReplace()
    {
        if (!stateMachine.Is(MenuTeamState.Idle) || isBattleMode) return;
        stateMachine.Set(MenuTeamState.Replacing);
    }

    private void HandleShortcutTeamCharacterNext()
    {
        if (!stateMachine.Is(MenuTeamState.Idle)) return;
        UIEvents.RaiseCharacterDetailSideNextPageRequested();
    }

    #endregion

    #region Button Handlers

    public void OnButtonSetActiveClicked()
    {
        if (!isEditMode) return;
        if (teamManager.ActiveLoadoutGuid == currentTeam.TeamGuid) return;
        teamManager.SetActiveLoadout(currentTeam.TeamGuid);
        UpdateSetActiveButtonState();
    }

    public void OnButtonCloseClicked()
    {
        UIEvents.RaiseBackFromTeamRequested(currentTeam, hasSwapped);
        RequestClose();
    }

    public void OnButtonTeamActionsClicked()
        => UIEvents.RaiseTeamActionsOpened(currentTeam, currentBattleType);

    public void OnButtonDeleteClicked()
        => UIEvents.RaiseTeamPanelDeleteOpened(currentTeam);

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();

        UIEvents.OnTeamLoadoutSelected                       += HandleLoadoutSelected;
        UIEvents.OnMenuTeamBattleRequested                   += HandleMenuTeamBattleRequested;
        UIEvents.OnFormationCharacterSlotUISelectedDefault   += HandleFormationCharacterSlotUISelectedDefault;
        UIEvents.OnFormationCharacterSlotUIClicked           += HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISelected          += HandleFormationCharacterSlotUISelected;
        UIEvents.OnFormationCharacterSlotUIHighlighted       += HandleFormationCharacterSlotUIHighlighted;
        UIEvents.OnFormationCharacterSlotUISwapped           += HandleFormationCharacterSlotUISwapped;
        UIEvents.OnFormationCharacterSlotUIReplaced          += HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIMoveRequested     += HandleFormationCharacterSlotUIMoveRequested;
        UIEvents.OnFormationCharacterSlotUIMoveCanceled      += HandleFormationCharacterSlotUIMoveCanceled;
        UIEvents.OnSelectorCharacterActionClicked            += HandleSelectorCharacterActionClicked;
        UIEvents.OnItemSelected                              += HandleItemSelected;
        UIEvents.OnSelectorItemSideListItemSelected          += HandleSelectorItemSideListItemSelected;
        UIEvents.OnSelectorItemSideListItemHighlighted       += HandleSelectorItemSideListItemHighlighted;
        UIEvents.OnBackFromSelectorItemSideRequested         += HandleBackFromSelectorItemSideRequested;
        UIEvents.OnBattleTypeChangeRequested                 += HandleBattleTypeChangeRequested;
        UIEvents.OnTeamEmblemChanged                         += HandleTeamEmblemChanged;
        UIEvents.OnTeamNameChanged                           += HandleTeamNameChanged;
        UIEvents.OnTeamActionsClosed                         += HandleTeamActionsClosed;
        UIEvents.OnTeamCharacterActionsClosed                += HandleTeamCharacterActionsClosed;
        UIEvents.OnSubstitutionChangesUpdated                += HandleSubstitutionChangesUpdated;
        UIEvents.OnBackFromCharacterSelectorRequested        += HandleBackFromCharacterSelectorRequested;
        TeamEvents.OnLoadoutDeleted                          += HandleLoadoutDeleted;
        BattleEvents.OnBattleStart                           += HandleBattleStart;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        UIEvents.OnTeamLoadoutSelected                       -= HandleLoadoutSelected;
        UIEvents.OnMenuTeamBattleRequested                   -= HandleMenuTeamBattleRequested;
        UIEvents.OnFormationCharacterSlotUISelectedDefault   -= HandleFormationCharacterSlotUISelectedDefault;
        UIEvents.OnFormationCharacterSlotUIClicked           -= HandleFormationCharacterSlotUIClicked;
        UIEvents.OnFormationCharacterSlotUISelected          -= HandleFormationCharacterSlotUISelected;
        UIEvents.OnFormationCharacterSlotUIHighlighted       -= HandleFormationCharacterSlotUIHighlighted;
        UIEvents.OnFormationCharacterSlotUISwapped           -= HandleFormationCharacterSlotUISwapped;
        UIEvents.OnFormationCharacterSlotUIReplaced          -= HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIMoveRequested     -= HandleFormationCharacterSlotUIMoveRequested;
        UIEvents.OnFormationCharacterSlotUIMoveCanceled      -= HandleFormationCharacterSlotUIMoveCanceled;
        UIEvents.OnSelectorCharacterActionClicked            -= HandleSelectorCharacterActionClicked;
        UIEvents.OnItemSelected                              -= HandleItemSelected;
        UIEvents.OnSelectorItemSideListItemSelected          -= HandleSelectorItemSideListItemSelected;
        UIEvents.OnSelectorItemSideListItemHighlighted       -= HandleSelectorItemSideListItemHighlighted;
        UIEvents.OnBackFromSelectorItemSideRequested         -= HandleBackFromSelectorItemSideRequested;
        UIEvents.OnBattleTypeChangeRequested                 -= HandleBattleTypeChangeRequested;
        UIEvents.OnTeamEmblemChanged                         -= HandleTeamEmblemChanged;
        UIEvents.OnTeamNameChanged                           -= HandleTeamNameChanged;
        UIEvents.OnTeamActionsClosed                         -= HandleTeamActionsClosed;
        UIEvents.OnTeamCharacterActionsClosed                -= HandleTeamCharacterActionsClosed;
        UIEvents.OnSubstitutionChangesUpdated                -= HandleSubstitutionChangesUpdated;
        UIEvents.OnBackFromCharacterSelectorRequested        -= HandleBackFromCharacterSelectorRequested;
        TeamEvents.OnLoadoutDeleted                          -= HandleLoadoutDeleted;
        BattleEvents.OnBattleStart                           -= HandleBattleStart;
    }

    // --- Open requests ---

    private void HandleLoadoutSelected(Team team)
    {
        currentTeam = team;
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleMenuTeamBattleRequested(Team team)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;
        currentTeam = team;
        MenuManager.Instance.OpenMenu(this);
    }

    // --- Slot interaction ---

    private void HandleFormationCharacterSlotUISelectedDefault(FormationCharacterSlotUI slot)
    {
        if (!MenuManager.Instance.IsMenuOpen(this) || slot == null) return;

        UIEvents.RaiseCharacterDetailSideUpdateRequested(slot.GetCharacter(), slot.FormationCoord.Position);
        SetDefaultSelectable(slot.Button);
        UIEvents.RaiseFormationCharacterSlotUISelected(slot);
    }

    private void HandleFormationCharacterSlotUIClicked(FormationCharacterSlotUI slot)
    {
        if (!IsInteractable() || slot == null) return;

        if (stateMachine.Is(MenuTeamState.Swapping))
        {
            if (pickedSlot != slot)
                UIEvents.RaiseFormationCharacterSlotUISwapped(pickedSlot, slot);

            stateMachine.Set(MenuTeamState.Idle);
            return;
        }

        currentSlot  = slot;
        selectedSlot = slot;
        UIEvents.RaiseTeamCharacterActionsOpenRequested(slot.GetCharacter());
    }

    private void HandleFormationCharacterSlotUISelected(FormationCharacterSlotUI slot)
    {
        if (!IsInteractable()) return;
        selectedSlot = slot;
    }

    private void HandleFormationCharacterSlotUIHighlighted(FormationCharacterSlotUI slot)
    {

    }

    // --- Move / Swap ---

    private void HandleFormationCharacterSlotUIMoveRequested(FormationCharacterSlotUI _)
    {
        if (!stateMachine.Is(MenuTeamState.Idle)) return;
        if (selectedSlot == null) return;
        stateMachine.Set(MenuTeamState.Swapping);
    }

    private void HandleFormationCharacterSlotUIMoveCanceled(FormationCharacterSlotUI _)
    {
        if (!stateMachine.Is(MenuTeamState.Swapping)) return;
        stateMachine.Set(MenuTeamState.Idle);
        audioManager.PlaySfx("sfx-menu_cancel");
    }

    private void HandleFormationCharacterSlotUISwapped(FormationCharacterSlotUI a, FormationCharacterSlotUI b)
    {
        bool isValidSwap = isEditMode || substitutionManager.ValidateSwap(currentTeam.TeamSide, a, b);
        if (!isValidSwap) return;

        hasSwapped = true;

        string guidA = a.GetCharacter().CharacterGuid;
        string guidB = b.GetCharacter().CharacterGuid;

        if (isBattleMode)
        {
            teamManager.SwapCharactersInBattle(
                currentTeam, currentBattleType,
                a.SlotIndex, a.FormationCoord, guidA,
                b.SlotIndex, b.FormationCoord, guidB);
        }

        if (isEditMode)
        {
            a.GetCharacter().ApplyKit(currentTeam.Kit, currentTeam.Variant, b.FormationCoord.Position);
            b.GetCharacter().ApplyKit(currentTeam.Kit, currentTeam.Variant, a.FormationCoord.Position);

            teamManager.SetCharacterInLoadout(currentTeam, currentBattleType, a.SlotIndex, guidB);
            teamManager.SetCharacterInLoadout(currentTeam, currentBattleType, b.SlotIndex, guidA);
        }

        // Visual swap
        Character temp = a.GetCharacter();
        a.SetCharacter(b.GetCharacter());
        b.SetCharacter(temp);

        UIEvents.RaiseCharacterDetailSideUpdateRequested(b.GetCharacter(), b.FormationCoord.Position);
        SetDefaultSelectable(b.Button);

        if (stateMachine.Is(MenuTeamState.Swapping))
            stateMachine.Set(MenuTeamState.Idle);
    }

    // --- Replace ---

    private void HandleFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character)
    {
        if (!MenuManager.Instance.IsMenuOpen(this) || character == null || slot == null) return;

        string newGuid = character.CharacterGuid;

        if (currentTeam.ContainsCharacterGuid(newGuid, currentBattleType))
        {
            FormationCharacterSlotUI other = formationLayoutUI.FindSlotByCharacterGuid(newGuid);
            if (other != null && other != slot)
                UIEvents.RaiseFormationCharacterSlotUISwapped(slot, other);
        }
        else
        {
            if (isEditMode)
                character.ApplyKit(currentTeam.Kit, currentTeam.Variant, slot.FormationCoord.Position);

            teamManager.SetCharacterInLoadout(currentTeam, currentBattleType, slot.SlotIndex, newGuid);
            slot.SetCharacter(character);
        }

        UIEvents.RaiseCharacterDetailSideUpdateRequested(slot.GetCharacter(), slot.FormationCoord.Position);
        SetDefaultSelectable(slot.Button);
    }

    private void HandleSelectorCharacterActionClicked(Character character)
    {
        if (!stateMachine.Is(MenuTeamState.Replacing)) return;

        UIEvents.RaiseFormationCharacterSlotUIReplaced(selectedSlot, character);
        stateMachine.Set(MenuTeamState.Idle);
    }

    private void HandleBackFromCharacterSelectorRequested()
    {
        if (stateMachine.Is(MenuTeamState.Replacing))
            stateMachine.Set(MenuTeamState.Idle);
    }

    // --- Item / Formation / Kit ---

    private void HandleItemSelected(Item item)
    {
        if (!MenuManager.Instance.IsMenuOpen(this) || !isEditMode) return;

        if (item.Category == ItemCategory.Formation)
        {
            cachedItemFormation = item as ItemFormation;
            cachedFormation     = formationDatabase.GetFormation(cachedItemFormation.FormationId);
            currentTeam.SetFormation(cachedFormation, currentBattleType);
            formationLayoutUI.SetFormation(cachedFormation);
        }
        else if (item.Category == ItemCategory.Kit)
        {
            cachedItemKit = item as ItemKit;
            cachedKit     = kitDatabase.GetKit(cachedItemKit.KitId);
            currentTeam.SetKit(cachedKit);
            formationLayoutUI.SetKit(cachedKit, false);
        }

        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void PreviewSelectorItem(SelectorItemSideListItem listItem)
    {
        if (!MenuManager.Instance.IsMenuOpen(this) || !isEditMode) return;

        if (listItem.ItemStorageSlot.Item.Category == ItemCategory.Formation)
        {
            cachedItemFormation = listItem.ItemStorageSlot.Item as ItemFormation;
            cachedFormation     = formationDatabase.GetFormation(cachedItemFormation.FormationId);
            formationLayoutUI.SetFormation(cachedFormation, false);
        }
        else if (listItem.ItemStorageSlot.Item.Category == ItemCategory.Kit)
        {
            cachedItemKit = listItem.ItemStorageSlot.Item as ItemKit;
            cachedKit     = kitDatabase.GetKit(cachedItemKit.KitId);
            formationLayoutUI.SetKit(cachedKit, false);
        }
    }

    private void HandleSelectorItemSideListItemHighlighted(SelectorItemSideListItem listItem)
        => PreviewSelectorItem(listItem);

    private void HandleSelectorItemSideListItemSelected(SelectorItemSideListItem listItem)
        => PreviewSelectorItem(listItem);

    private void HandleBackFromSelectorItemSideRequested(ItemCategory category)
    {
        if (!MenuManager.Instance.IsMenuOpen(this) || !isEditMode) return;

        if (category == ItemCategory.Formation)
            formationLayoutUI.SetFormation(currentTeam.GetFormation(currentBattleType), false);
        else if (category == ItemCategory.Kit)
            formationLayoutUI.SetKit(currentTeam.Kit, false);
    }

    // --- Battle / Team data updates ---

    private void HandleBattleTypeChangeRequested()
    {
        if (!isEditMode) return;

        BattleType oldType = currentBattleType;
        currentBattleType  = (currentBattleType == BattleType.Full) ? BattleType.Mini : BattleType.Full;

        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode);
        teamManager.SetDefaultBattleType(currentBattleType);
        UIEvents.RaiseBattleTypeChanged(currentBattleType, oldType);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void HandleTeamEmblemChanged(string emblemId)
    {
        if (!isEditMode) return;
        currentTeam.UpdateAppearance(emblemId);
        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode, false);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    private void HandleTeamNameChanged(string newName)
    {
        if (!isEditMode) return;
        currentTeam.SetCustomName(newName);
        formationLayoutUI.Initialize(currentTeam, currentBattleType, mode, false);
        UIEvents.RaiseBackFromTeamActionsRequested();
    }

    // --- Submenu close return-focus ---

    private void HandleTeamActionsClosed()
    {
        // Memory restoration is handled by the base class.
        SetDefaultFocus();
    }

    private void HandleTeamCharacterActionsClosed()
    {

    }

    // --- Misc ---

    private void HandleLoadoutDeleted(Team _) => RequestClose();

    private void HandleSubstitutionChangesUpdated(int currentValue, int maxValue)
        => UpdateChangesText(currentValue, maxValue);

    private void HandleBattleStart(BattleType battleType)
        => currentBattleType = battleType;

    #endregion
}
