using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Input;

public class MenuTeamPanelTeamActions : Menu
{
    #region Fields

    //[Header("UI References")]
    //[SerializeField] private MenuTeamMode mode;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;
    private AudioManager audioManager;

    private Team team;
    private BattleType battleType;
    private bool isClosing = false;

    #endregion

    #region Lifecycle

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
        audioManager = AudioManager.Instance;
    }

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        isClosing = false;

        base.Show();
        base.SetInteractable(true);
    }

    public override void Hide()
    {
        isClosing = false;

        base.SetInteractable(false);
        base.Hide();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);

        if (interactable && isClosing)
        {
            isClosing = false;
            Close();
        }

        if (interactable) 
            SubscribeInput();
        else
            UnsubscribeInput();
    }

    public void Close()
    {
        if (!isTop) return;
        menuManager.CloseMenu();
        UIEvents.RaiseTeamActionsClosed();
    }

    #endregion

    #region Logic

    #endregion

    #region Helper

    #endregion

    #region Input 

    private void SubscribeInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, Close);
    }

    private void UnsubscribeInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, Close);
    }

    #endregion

    #region Button Handle

    public void OnButtonChangeFormationClicked() 
    {
        UIEvents.RaiseItemSelectorSideOpened(ItemCategory.Formation, battleType);
    }

    public void OnButtonChangeKitClicked() 
    {
        UIEvents.RaiseItemSelectorSideOpened(ItemCategory.Kit, battleType);
    }

    public void OnButtonChangeNameClicked() 
    {
        UIEvents.RaiseTeamPanelNameOpened(team.TeamName);
    }

    public void OnButtonChangeEmblemClicked() 
    {
        UIEvents.RaiseTeamPanelEmblemOpened(team.TeamCrestSprite);
    }

    public void OnButtonChangeBattleTypeClicked() 
    {
        UIEvents.RaiseBattleTypeChangeRequested();
    }

    public void OnButtonBackClicked() 
    {
        Close();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamActionsOpened += HandleTeamActionsOpened;
        UIEvents.OnBackFromTeamActionsRequested += HandleBackFromTeamActionsRequested;
        UIEvents.OnBattleTypeChanged += HandleBattleTypeChanged;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamActionsOpened -= HandleTeamActionsOpened;
        UIEvents.OnBackFromTeamActionsRequested -= HandleBackFromTeamActionsRequested;
        UIEvents.OnBattleTypeChanged -= HandleBattleTypeChanged;
    }

    private void HandleTeamActionsOpened(Team team, BattleType battleType) 
    {
        this.team = team;
        this.battleType = battleType;
        menuManager.OpenMenu(this);
    }

    private void HandleBackFromTeamActionsRequested() 
    {
        if (isTop)
            Close();
        else
            isClosing = true;
    }

    private void HandleBattleTypeChanged(BattleType currentBattleType, BattleType oldType) 
    {
        if (!isTop) return;
        this.battleType = currentBattleType;
    }

    #endregion
}
