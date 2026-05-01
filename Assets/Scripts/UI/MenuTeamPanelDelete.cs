using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuTeamPanelDelete : Menu
{
    #region Fields

    //[Header("UI References")]
    //[SerializeField] private TMP_InputField inputFieldName;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;

    private Team team;

    #endregion

    #region Lifecycle

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
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

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);

        if (interactable) 
            SubscribeInput();
        else
            UnsubscribeInput();
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

    #region Input 

    private void SubscribeInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);
    }

    private void UnsubscribeInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);
    }

    #endregion

    #region Button Handle

    public void OnButtonConfirmClicked() 
    {
        UIEvents.RaiseTeamLoadoutDeleteRequested(team);
        Close();
    }

    public void OnButtonCancelClicked() 
    {
        Close();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamPanelDeleteOpened += HandleTeamPanelDeleteOpened;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamPanelDeleteOpened -= HandleTeamPanelDeleteOpened;
    }

    private void HandleTeamPanelDeleteOpened(Team team) 
    {
        this.team = team;
        menuManager.OpenMenu(this);
    }

    #endregion
}
