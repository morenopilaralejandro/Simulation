using UnityEngine;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorScoutEntry : Selector<ScoutEntry, SelectorScoutEntryListItem>
{
    #region Fields

    [Header("References")]
    [SerializeField] private TMP_Text textHeader;
    [SerializeField] private TMP_Text textGold;
    private int selectedIndex;

    //private CharacterFilterData activeFilterData;

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        // Other code here
        base.Show();
        if (active.Count > 0) 
        {
            UIEvents.RaiseCharacterDetailSideUpdateRequested(
                active[0].Character,
                active[0].Character.Position);
        } 
    }

    public override void SetInteractable(bool interactable)
    {
        FocusItem(selectedIndex);   
        base.SetInteractable(interactable);
    }

    public override void Hide()
    {
        // Reset filter UI when closing.
        // UIEvents.RaiseCharacterFilterResetRequested();
        // activeFilterData = null;

        selectedIndex = -1;

        base.Hide();
    }

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back, HandleBack);
        im.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext, HandleShortcutTeamCharacterNext);
        //im.SubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter, HandleFilterShortcut);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back, HandleBack);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext, HandleShortcutTeamCharacterNext);
        //im.UnsubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter,       HandleFilterShortcut);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorScoutEntryListItem view, ScoutEntry data)
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
        AudioManager.Instance.PlaySfxUI("sfx-menu_back");
        RequestClose();
    }

    private void HandleShortcutTeamCharacterNext()
    {
        UIEvents.RaiseCharacterDetailSideNextPageRequested();
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
        UIEvents.OnScoutEntrySelectorOpenRequested += HandleOpenRequested;
        UIEvents.OnScoutEntrySelectorRefreshRequested += HandleRefreshRequested;
        UIEvents.OnSelectorScoutEntryActionClicked += HandleSelectorScoutEntryActionClicked;

        //UIEvents.OnCharacterFilterUpdated += HandleFilterUpdated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnScoutEntrySelectorOpenRequested -= HandleOpenRequested;
        UIEvents.OnScoutEntrySelectorRefreshRequested -= HandleRefreshRequested;
        UIEvents.OnSelectorScoutEntryActionClicked -= HandleSelectorScoutEntryActionClicked;
        //UIEvents.OnCharacterFilterUpdated -= HandleFilterUpdated;
    }

    private void HandleOpenRequested(
        ISelectorSource<ScoutEntry>      source,
        ISelectorClickAction<ScoutEntry> action,
        ISelectorFilter<ScoutEntry>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        if (source is SelectorScoutEntrySourceFromTier tierSource)
        {
            textHeader.text = tierSource.ScoutTier.ScoutTierName;
        }

        textGold.text = ItemManager.Instance.GetGold().ToString();

        Open(source, action, filter);
    }

    private void HandleRefreshRequested()
    {
        Refresh();
        textGold.text = ItemManager.Instance.GetGold().ToString();
    }

    private void HandleSelectorScoutEntryActionClicked(ScoutEntry scoutEntry)
    {
        if (scoutEntry.IsOwned || !scoutEntry.IsAffordable)
        {
            AudioManager.Instance.PlaySfxUI("sfx-menu_forbidden");
            return;
        }
        
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        selectedIndex = GetSelectedIndex();
        UIEvents.RaiseMenuScoutConfirmOpenRequested(scoutEntry);
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
