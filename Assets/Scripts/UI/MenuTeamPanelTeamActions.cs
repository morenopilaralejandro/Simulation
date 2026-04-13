using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;
using Simulation.Enums.UI;
using Simulation.Enums.Item;

public class MenuTeamPanelTeamActions : Menu
{
    #region Fields

    //[Header("UI References")]
    //[SerializeField] private MenuTeamMode mode;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
    }

    private void OnDestroy()
    {

    }

    #endregion

   #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);
    }

    public override void Hide()
    {
        base.SetInteractable(false);
        base.Hide();
    }

    public void Close()
    {
        if (!isOpen) return;
        menuManager.CloseMenu();
    }

    #endregion

    #region Logic

    #endregion

    #region Helper


    #endregion

    #region Button Handle

    public void OnButtonChangeFormationClicked() 
    {
        UIEvents.RaiseItemSelectorSideOpened(ItemCategory.Formation);
    }

    public void OnButtonChangeKitClicked() 
    {
        UIEvents.RaiseItemSelectorSideOpened(ItemCategory.Kit);
    }

    public void OnButtonChangeNameClicked() 
    {
        UIEvents.RaiseTeamPanelNameOpened();
    }

    public void OnButtonChangeEmblemClicked() 
    {
        UIEvents.RaiseTeamPanelEmblemOpened();
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
    }

    private void OnDisable()
    {
        UIEvents.OnTeamActionsOpened -= HandleTeamActionsOpened;
    }

    private void HandleTeamActionsOpened() 
    {
        Debug.LogError("open");
        menuManager.OpenMenu(this);
    }

    #endregion
}
