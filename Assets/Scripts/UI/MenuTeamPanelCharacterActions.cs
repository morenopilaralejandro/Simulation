using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;

public class MenuTeamPanelCharacterActions : Menu
{
    #region Fields

    //[Header("UI References")]
    //[SerializeField] private MenuTeamMode mode;

    private Character character;

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

    public void OnButtonSummaryClicked() 
    {
        UIEvents.RaiseCharacterDetailOpened();
    }

    public void OnButtonMoveClicked() 
    {
        Close();
        UIEvents.RaiseFormationCharacterSlotUIMoveRequested(null);
    }

    public void OnButtonReplaceClicked() 
    {
        UIEvents.RaiseCharacterSelectorOpened(null, default);
    }

    public void OnButtonBackClicked() 
    {
        Close();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnCharacterActionsOpened += HandleCharacterActionsOpened;
    }

    private void OnDisable()
    {
        UIEvents.OnCharacterActionsOpened -= HandleCharacterActionsOpened;
    }

    private void HandleFormationCharacterSlotUIClicked(Character character) 
    {
        this.character = character;
    }

    private void HandleCharacterActionsOpened() 
    {
        menuManager.OpenMenu(this);
    }

    #endregion
}
