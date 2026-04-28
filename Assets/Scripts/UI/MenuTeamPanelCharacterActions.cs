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
    private bool isReplacing = false;
    private bool isClosing = false;

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
    }

    public void Close()
    {
        if (!isTop) return;
        menuManager.CloseMenu();
        UIEvents.RaiseTeamCharacterActionsClosed();
    }

    #endregion

    #region Logic

    #endregion

    #region Helper


    #endregion

    #region Button Handle

    public void OnButtonSummaryClicked() 
    {
        Close();
        UIEvents.RaiseCharacterDetailOpenRequested(character);
    }

    public void OnButtonMoveClicked() 
    {
        Close();
        UIEvents.RaiseFormationCharacterSlotUIMoveRequested(null);
    }

    public void OnButtonReplaceClicked() 
    {
        isReplacing = true;
        UIEvents.RaiseFormationCharacterSlotUIReplaceRequested();
        UIEvents.RaiseCharacterSelectorOpenRequested(CharacterSelectorMode.GetFromStorage, null, default, true);
    }

    public void OnButtonBackClicked() 
    {
        Close();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnTeamCharacterActionsOpenRequested += HandleTeamCharacterActionsOpenRequested;
        UIEvents.OnCharacterSelected += HandleCharacterSelected;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamCharacterActionsOpenRequested -= HandleTeamCharacterActionsOpenRequested;
        UIEvents.OnCharacterSelected -= HandleCharacterSelected;
    }

    private void HandleTeamCharacterActionsOpenRequested(Character character) 
    {
        if (isOpen) return;
        this.character = character;
        menuManager.OpenMenu(this);
    }

    private void HandleCharacterSelected(Character character) 
    {
        if (isReplacing) 
        {
            isReplacing = false;
            if (isTop)
                Close();
            else
                isClosing = true;
        }
    }

    #endregion
}
