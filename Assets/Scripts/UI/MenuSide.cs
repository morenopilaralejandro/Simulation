using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.World;

/*
    menu things
    MenuManager.Instance.ReplaceMenu(duelLogMenu);
*/

public class MenuSide : Menu
{
    [SerializeField] private MenuTeam menuTeam;
    [SerializeField] private MenuSave menuSave;

    private MenuManager menuManager;
    private WorldManager worldManager;
    private bool isOpen => menuManager.IsMenuOpen(this);
    private bool isTop => menuManager.IsMenuOnTop(this);
    public bool IsSideMenuOpen => isOpen;

    private void Awake()
    {
        //BattleUIManager.Instance.RegisterBattleMenu(this);
    }

    private void OnDestroy()
    {
        //BattleUIManager.Instance.UnregisterBattleMenu(this);
    }

    void Start()
    {
        menuManager = MenuManager.Instance;
        worldManager = WorldManager.Instance;
        base.Hide();
        base.SetInteractable(false);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (isTop)
        {
            if (InputManager.Instance.GetDown(CustomAction.World_CloseSideMenu))
                Close();
        } else 
        {
            if (!WorldManager.Instance.PlayerWorldEntity.CanOpenMenu) return;
            if (InputManager.Instance.GetDown(CustomAction.World_OpenSideMenu))
                Open();
        }
    }

    public override void Show()
    {
        base.Show();
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        base.Hide();
    }

    public void Open()
    {
        if (menuManager.CurrentMenu != null && 
            !menuManager.IsMenuOnTop(this)) 
            return;

        if (isOpen) return;

        menuManager.OpenMenu(this);
        worldManager.PlayerWorldEntity.SetState(PlayerWorldState.InMenu);
    }

    public void Close()
    {
        if (!menuManager.IsMenuOnTop(this)) return;
        if (!isOpen) return;
        menuManager.CloseMenu();
        worldManager.PlayerWorldEntity.SetState(PlayerWorldState.FreeRoam);
    }

    public void OnButtonTapped()
    {
        Close();
    }

    public void OnButtonTeamTapped()
    {
        menuManager.OpenMenu(menuTeam);
    }

    public void OnButtonSaveTapped()
    {
        menuManager.OpenMenu(menuSave);
    }

}
