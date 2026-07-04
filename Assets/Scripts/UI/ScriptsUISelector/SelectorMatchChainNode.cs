using UnityEngine;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorMatchChainNode : Selector<MatchChainNode, SelectorMatchChainNodeListItem>
{
    #region Fields

    [Header("Visuals")]
    [SerializeField] private TMP_Text textName;
    private int selectedIndex;

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();

        FocusItem(selectedIndex);
    }

    public override void Hide()
    {
        base.Hide();
    }

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorMatchChainNodeListItem view, MatchChainNode data)
    {
        view.Bind(data);
    }

    #endregion

    #region Public API

    #endregion

    #region Input

    private void HandleBack()
    {
        //UIEvents.RaiseBackFromCharacterSelectorRequested();
        RequestClose();
        DialogEvents.RaiseDialogMenuClosed();
    }

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMatchChainNodeSelectorOpenRequested += HandleOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMatchChainNodeSelectorOpenRequested -= HandleOpenRequested;
    }

    private void HandleOpenRequested(
        ISelectorSource<MatchChainNode>      source,
        ISelectorClickAction<MatchChainNode> action,
        ISelectorFilter<MatchChainNode>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;

        if (source is SelectorMatchChainNodeSource s) 
        {
            textName.text = s.MatchChain.MatchChainName;
            selectedIndex = s.MatchChain.SelectedIndex;
        }

        Open(source, action, filter);
    }

    #endregion
}
