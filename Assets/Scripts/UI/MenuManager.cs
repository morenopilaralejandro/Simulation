using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private Stack<Menu> menuStack = new Stack<Menu>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BattleEvents.OnBattleEnd += CloseAllMenus;
    }

    void OnDestroy()
    {
        BattleEvents.OnBattleEnd -= CloseAllMenus;
    }

    public void OpenMenu(Menu menu)
    {
        if (menuStack.Count > 0)
            menuStack.Peek().SetInteractable(false);

        menuStack.Push(menu);
        menu.Show();
        menu.SetInteractable(true);
        UIEvents.RaiseMenuOpened(menu);
    }

    public void ReplaceMenu(Menu newMenu)
    {
        // Close the current top menu
        if (menuStack.Count > 0)
        {
            Menu top = menuStack.Pop();
            top.Hide();
            top.SetInteractable(false);
            UIEvents.RaiseMenuClosed(top);
        }

        // Open the new one
        menuStack.Push(newMenu);
        newMenu.Show();
        newMenu.SetInteractable(true);
        UIEvents.RaiseMenuOpened(newMenu);
    }

    public void CloseMenu()
    {
        if (menuStack.Count == 0)
            return;

        Menu top = menuStack.Pop();
        top.Hide();
        top.SetInteractable(false);
        UIEvents.RaiseMenuClosed(top);

        if (top.CloseAllPrevious)
        {
            CloseAllMenus();
            return;
        }

        if (menuStack.Count > 0)
            menuStack.Peek().SetInteractable(true);
        else
            UIEvents.RaiseAllMenusClosed();
    }

    public void CloseAllMenus()
    {
        while (menuStack.Count > 0)
        {
            Menu m = menuStack.Pop();
            m.Hide();
        }
        UIEvents.RaiseAllMenusClosed();
    }

    public bool IsMenuOnTop(Menu menu) => menuStack.Count > 0 && menuStack.Peek() == menu;
    public bool IsMenuOpen(Menu menu) => menuStack.Contains(menu);
    public Menu CurrentMenu => menuStack.Count > 0 ? menuStack.Peek() : null;

}
