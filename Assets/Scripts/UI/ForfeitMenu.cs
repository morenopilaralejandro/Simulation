using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class ForfeitMenu : Menu
{
    public bool IsForfeitMenuOpen => MenuManager.Instance.IsMenuOpen(this);

    private void Awake()
    {
        BattleUIManager.Instance.RegisterForfeitMenu(this);
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterForfeitMenu(this);
    }

    void Start()
    {
        base.Hide();
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnButtonConfimTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        BattleManager.Instance.ForfeitBattle();
        MenuManager.Instance.CloseMenu();
    }

    public void OnButtonCancelTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        MenuManager.Instance.CloseMenu();
    }

    public void OnButtonSelected() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_change");
    }

}
