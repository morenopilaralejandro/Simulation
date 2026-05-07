using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorLoadout : Selector<Team, SelectorLoadoutListItem>
{
    #region Fields

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        // extra logic        

        base.Show();
    }

    public override void Hide()
    {
        // Reset filter UI when closing.
        base.Hide();
    }

    protected override void OnGainedInput()
    {
        //InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    protected override void OnLostInput()
    {
        //InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorLoadoutListItem view, Team data)
    {
        // extra logic here

        view.Bind(data);
    }

    #endregion

    #region Public API

    #endregion

    #region Input

    /*

    private void HandleBack()
    {
        // UIEvents.RaiseBackFromMoveSelectorRequested();
        RequestClose();
    }

    */

    private SelectorLoadoutListItem GetLastSelectedItem()
    {
        var view = LastSelected.GetComponent<SelectorLoadoutListItem>();
        return view;
    }

    #endregion

    #region Buttons

    // public void OnButtonBackClicked() => HandleBack();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnLoadoutSelectorOpenRequested += HandleOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnLoadoutSelectorOpenRequested -= HandleOpenRequested;
    }

    private void HandleOpenRequested(
        ISelectorSource<Team>      source,
        ISelectorClickAction<Team> action,
        ISelectorFilter<Team>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        Open(source, action, filter);
    }

    #endregion

}
