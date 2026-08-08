using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorScoutTier : Selector<ScoutTier, SelectorScoutTierListItem>
{
    #region Fields

    //[Header("References")]
    //[SerializeField] private Variant defaultVariant = Variant.Home;

    //private CharacterFilterData activeFilterData;

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        // Other code here
        base.Show();
    }

    public override void Hide()
    {
        // Reset filter UI when closing.
        // UIEvents.RaiseCharacterFilterResetRequested();
        // activeFilterData = null;

        base.Hide();
    }

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back,                            HandleBack);
        //im.SubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter,         HandleFilterShortcut);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back,                          HandleBack);
        //im.UnsubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter,       HandleFilterShortcut);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorScoutTierListItem view, ScoutTier data)
    {
        // Other code here
        view.Bind(data);
    }

    #endregion

    #region Public API

    #endregion

    #region Input

    private void HandleBack()
    {
        RequestClose();
    }

    /*

    private void HandleFilterShortcut()
    {
        UIEvents.RaiseCharacterFilterRequested();
    }

    */

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();
    //public void OnButtonFilterClicked() => HandleFilterShortcut();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnScoutTierSelectorOpenRequested += HandleOpenRequested;
        //UIEvents.OnCharacterFilterUpdated += HandleFilterUpdated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnScoutTierSelectorOpenRequested -= HandleOpenRequested;
        //UIEvents.OnCharacterFilterUpdated -= HandleFilterUpdated;
    }

    private void HandleOpenRequested(
        ISelectorSource<ScoutTier>      source,
        ISelectorClickAction<ScoutTier> action,
        ISelectorFilter<ScoutTier>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        Open(source, action, filter);
    }

    /*

    private void HandleFilterUpdated(CharacterFilterData data)
    {
        activeFilterData = data;
        ApplyFilter(new CharacterFilterAdapter(data));
    }

    */

    #endregion
}
