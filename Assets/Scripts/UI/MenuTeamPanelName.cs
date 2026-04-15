using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;
using Simulation.Enums.UI;

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
