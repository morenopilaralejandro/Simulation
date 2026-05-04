using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorCharacter : Selector<Character, SelectorCharacterListItem>
{
    #region Fields

    [Header("Visuals")]
    [SerializeField] private Variant defaultVariant = Variant.Home;
    [SerializeField] private Role    defaultRole    = Role.Field;

    private Kit     kit;
    private Variant variant;
    private Role    role;

    private CharacterFilterData activeFilterData;
    private bool isDetailShorcutAllow;

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        // Default kit/variant if none was injected by the open request.
        if (kit == null)
            kit = TeamManager.Instance.ActiveLoadout?.Kit;

        base.Show();
    }

    public override void Hide()
    {
        // Reset filter UI when closing.
        UIEvents.RaiseCharacterFilterResetRequested();
        activeFilterData = null;

        base.Hide();
    }

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back,                            HandleBack);
        im.SubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter,         HandleFilterShortcut);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterSummary,    HandleSummaryShortcut);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back,                          HandleBack);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter,       HandleFilterShortcut);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterSummary,  HandleSummaryShortcut);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorCharacterListItem view, Character data)
    {
        if (kit != null)
            data.ApplyKit(kit, variant, role);

        view.Bind(data);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Configure the visual kit applied to characters when bound.
    /// Call before opening, or pass through the open request.
    /// </summary>
    public void ConfigureKit(Kit kit, Variant variant, Role role)
    {
        this.kit     = kit;
        this.variant = variant;
        this.role    = role;
    }

    #endregion

    #region Input

    private void HandleBack()
    {
        UIEvents.RaiseBackFromCharacterSelectorRequested();
        RequestClose();
    }

    private void HandleFilterShortcut()
    {
        UIEvents.RaiseCharacterFilterRequested();
    }

    private void HandleSummaryShortcut()
    {
        if (!isDetailShorcutAllow) return;
        var item = GetLastSelectedItem();
        if (item == null || item.Data == null) return;
        UIEvents.RaiseCharacterDetailOpenRequested(item.Data);
    }

    private SelectorCharacterListItem GetLastSelectedItem()
    {
        var view = LastSelected.GetComponent<SelectorCharacterListItem>();
        return view;
    }

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();
    public void OnButtonFilterClicked() => HandleFilterShortcut();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnCharacterSelectorOpenRequested += HandleOpenRequested;
        UIEvents.OnCharacterFilterUpdated         += HandleFilterUpdated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnCharacterSelectorOpenRequested -= HandleOpenRequested;
        UIEvents.OnCharacterFilterUpdated         -= HandleFilterUpdated;
    }

    private void HandleOpenRequested(
        ISelectorSource<Character>      source,
        ISelectorClickAction<Character> action,
        ISelectorFilter<Character>      filter,
        bool                            closeAfterPick)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        // Default visuals if not previously configured.
        if (kit == null)
        {
            kit     = TeamManager.Instance.ActiveLoadout?.Kit;
            variant = defaultVariant;
            role    = defaultRole;
        }

        isDetailShorcutAllow = action is SelectorCharacterActionOpenDetail;
        closeOnSelect = closeAfterPick;
        Open(source, action, filter);
    }

    private void HandleFilterUpdated(CharacterFilterData data)
    {
        activeFilterData = data;
        ApplyFilter(new CharacterFilterAdapter(data));
    }

    #endregion
}
