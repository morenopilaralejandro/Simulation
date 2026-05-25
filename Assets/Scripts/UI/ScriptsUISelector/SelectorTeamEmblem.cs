using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorTeamEmblem : Selector<Emblem, SelectorTeamEmblemListItem>
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

    protected override void Bind(SelectorTeamEmblemListItem view, Emblem data)
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

    private SelectorTeamEmblemListItem GetLastSelectedItem()
    {
        var view = LastSelected.GetComponent<SelectorTeamEmblemListItem>();
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
        UIEvents.OnTeamEmblemSelectorOpenRequested += HandleOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamEmblemSelectorOpenRequested -= HandleOpenRequested;
    }

    private void HandleOpenRequested(
        ISelectorSource<Emblem>      source,
        ISelectorClickAction<Emblem> action,
        ISelectorFilter<Emblem>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        Open(source, action, filter);
    }

    #endregion
}
