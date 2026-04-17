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

public class MenuPauseOverworld : Menu
{
    private MenuManager menuManager;
    private WorldManager worldManager;
    private bool isOpen => menuManager.IsMenuOpen(this);
    public bool IsPauseOverworldOpen => isOpen;

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
        if (isOpen)
        {
            if (InputManager.Instance.GetDown(CustomAction.World_ClosePauseMenu))
                Close();
        } else 
        {
            if (InputManager.Instance.GetDown(CustomAction.World_OpenPauseMenu))
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
        if (!isOpen) return;
        menuManager.CloseMenu();
        worldManager.PlayerWorldEntity.SetState(PlayerWorldState.FreeRoam);
    }

    public void OnButtonTapped()
    {
        //AutoBattleManager.Instance.ToggleAutoBattle();
        Close();
    }

}
