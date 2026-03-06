using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Simulation.Enums.Input;

/*
    menu things
    MenuManager.Instance.ReplaceMenu(duelLogMenu);
*/

public class MenuPauseOverworld : Menu
{
    private MenuManager menuManager;
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
        base.Hide();
        base.SetInteractable(false);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!isOpen)
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_OpenBattleMenu))
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
    }

    public void Close()
    {
        if (!isOpen) return;
        menuManager.CloseMenu();
    }

    public void OnButtonTapped()
    {
        //AutoBattleManager.Instance.ToggleAutoBattle();
        Close();
    }

}
