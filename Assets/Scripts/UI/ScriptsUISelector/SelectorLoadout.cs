using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorLoadout : Selector<Team, SelectorLoadoutListItem>
{
    #region Fields

    [SerializeField] private TMP_Text loadoutCountText;
    [SerializeField] private Button createButton;

    private TeamManager teamManager;

    #endregion

    #region Menu Overrides

    public override void Start()
    {
        base.Start();
        teamManager = TeamManager.Instance;
    }

    public override void Show()
    {
        int count = teamManager.GetLoadoutCount();
        UpdateCountText(count);
        UpdateCreateButtonState(count);

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

    protected override void Bind(SelectorLoadoutListItem view, Team data)
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
        UIEvents.RaiseBackFromMoveSelectorRequested();
        RequestClose();
    }

    private SelectorLoadoutListItem GetLastSelectedItem()
    {
        var view = LastSelected.GetComponent<SelectorLoadoutListItem>();
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
        UIEvents.OnMoveSelectorOpenRequested += HandleOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMoveSelectorOpenRequested -= HandleOpenRequested;
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

    #region Logic

    private void UpdateCountText(int count)
    {
        loadoutCountText.text = $"({count} / {TeamManager.MAX_LOADOUTS})";
    }

    private void UpdateCreateButtonState(int count)
    {
        createButton.interactable = count < TeamManager.MAX_LOADOUTS;
    }

    #endregion
}
