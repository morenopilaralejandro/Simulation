using UnityEngine;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Input;

public class SelectorEquipment : Selector<ItemEquipment, SelectorEquipmentListItem>
{
    #region Fields

    #endregion

    #region Menu Overrides

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

    protected override void Bind(SelectorEquipmentListItem view, ItemEquipment data)
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
        RequestClose();
    }

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnEquipmentSelectorOpenRequested += HandleOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnEquipmentSelectorOpenRequested -= HandleOpenRequested;
    }

    private void HandleOpenRequested(
        ISelectorSource<ItemEquipment>      source,
        ISelectorClickAction<ItemEquipment> action,
        ISelectorFilter<ItemEquipment>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;
        Open(source, action, filter);
    }

    #endregion
}
