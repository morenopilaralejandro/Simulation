using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;
using Simulation.Enums.UI;

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

        selectedId = null;
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
