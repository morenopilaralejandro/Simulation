using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuTeamPanelEmblem : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private Image imageEmblem;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;

    private string selectedId;

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

        selectedId = null;
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

    public void OnButtonChangeClicked() 
    {
        UIEvents.RaiseEmblemSelectorOpened();
    }

    public void OnButtonConfirmClicked() 
    {
        if (selectedId != null)
            UIEvents.RaiseTeamEmblemChanged(selectedId);
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
        UIEvents.OnTeamPanelEmblemOpened += HandleTeamPanelEmblemOpened;
        UIEvents.OnTeamEmblemSelected += HandleTeamEmblemSelected;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamPanelEmblemOpened -= HandleTeamPanelEmblemOpened;
        UIEvents.OnTeamEmblemSelected -= HandleTeamEmblemSelected;
    }

    private void HandleTeamPanelEmblemOpened(Sprite emblemSprite) 
    {
        imageEmblem.sprite = emblemSprite;
        menuManager.OpenMenu(this);
    }

    private void HandleTeamEmblemSelected(string emblemId, Sprite emblemSprite) 
    {
        selectedId = emblemId;
        imageEmblem.sprite = emblemSprite;
    }

    #endregion
}
