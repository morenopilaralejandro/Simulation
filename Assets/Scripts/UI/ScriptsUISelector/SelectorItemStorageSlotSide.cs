using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorItemStorageSlotSide : Selector<ItemStorageSlot, SelectorItemStorageSlotSideListItem>
{
    #region Fields

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        // placeholder
        base.Show();
    }

    public override void Hide()
    {
        // Reset filter UI when closing.
        base.Hide();
    }

    protected override void OnGainedInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    protected override void OnLostInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorItemStorageSlotSideListItem view, ItemStorageSlot data)
    {
        // extra logic here

        view.Bind(data);
    }

    #endregion

    #region Public API

    #endregion

    #region Input

    private void HandleBack()
    {
        var view = GetLastSelectedItem();
        if (view == null)
            UIEvents.RaiseBackFromSelectorItemStorageSlotSideRequested(default);
        else
            UIEvents.RaiseBackFromSelectorItemStorageSlotSideRequested(view.Data.Item.Category);

        RequestClose();
    }

    private SelectorItemStorageSlotSideListItem GetLastSelectedItem()
    {
        var view = LastSelected.GetComponent<SelectorItemStorageSlotSideListItem>();
        return view;
    }

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnItemStorageSlotSideSelectorOpenRequested += HandleOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnItemStorageSlotSideSelectorOpenRequested -= HandleOpenRequested;
    }

    private void HandleOpenRequested(
        ISelectorSource<ItemStorageSlot>      source,
        ISelectorClickAction<ItemStorageSlot> action,
        ISelectorFilter<ItemStorageSlot>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        Open(source, action, filter);
    }

    #endregion
}
