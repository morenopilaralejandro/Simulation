using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuTeamPanelName : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private TMP_InputField inputFieldName;

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
        UIEvents.RaiseTeamNameChanged(inputFieldName.text);
        Close();
    }

    public void OnButtonCancelClicked() 
    {
        Close();
    }

    public void OnInputValueChanged(string currentText)
    {

    }

    public void OnInputEndEdit()
    {
        OnButtonConfirmClicked();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamPanelNameOpened += HandleTeamPanelNameOpened;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamPanelNameOpened -= HandleTeamPanelNameOpened;
    }

    private void HandleTeamPanelNameOpened(string teamName) 
    {
        inputFieldName.text = teamName;
        menuManager.OpenMenu(this);
    }

    #endregion
}
