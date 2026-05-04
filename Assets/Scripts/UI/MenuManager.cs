using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    #region Singleton

    public static MenuManager Instance { get; private set; }

    private void Awake()
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

    private void OnDestroy()
    {
        BattleEvents.OnBattleEnd -= CloseAllMenus;
        if (Instance == this) Instance = null;
    }

    #endregion

    #region Fields

    private readonly Stack<Menu>   menuStack    = new();
    private readonly HashSet<Menu> pendingClose = new();

    public Menu CurrentMenu => menuStack.Count > 0 ? menuStack.Peek() : null;
    public int  Count       => menuStack.Count;

    #endregion

    #region Open

    public void OpenMenu(Menu menu)
    {
        if (menu == null) return;
        if (menuStack.Contains(menu)) return; // prevent double-push

        if (menuStack.Count > 0)
        {
            var prev = menuStack.Peek();
            prev.SetInteractable(false);
            prev.OnCovered();
        }

        menuStack.Push(menu);
        menu.Show();
        menu.SetInteractable(true);
        menu.OnOpened();
        UIEvents.RaiseMenuOpened(menu);
    }

    public void ReplaceMenu(Menu newMenu)
    {
        if (newMenu == null) return;

        if (menuStack.Count > 0)
        {
            Menu top = menuStack.Pop();
            pendingClose.Remove(top);
            top.SetInteractable(false);
            top.Hide();
            top.OnClosed();
            UIEvents.RaiseMenuClosed(top);
        }

        menuStack.Push(newMenu);
        newMenu.Show();
        newMenu.SetInteractable(true);
        newMenu.OnOpened();
        UIEvents.RaiseMenuOpened(newMenu);
    }

    #endregion

    #region Close

    /// <summary>
    /// Closes the menu immediately if it's on top, otherwise queues it to close
    /// once it becomes the top of the stack.
    /// </summary>
    public void RequestClose(Menu menu)
    {
        if (menu == null) return;
        if (!menuStack.Contains(menu)) return;

        if (menuStack.Peek() == menu)
            CloseMenu();
        else
            pendingClose.Add(menu);
    }

    public void CloseMenu()
    {
        if (menuStack.Count == 0) return;

        Menu top = menuStack.Pop();
        pendingClose.Remove(top);

        top.SetInteractable(false);
        top.Hide();
        top.OnClosed();
        UIEvents.RaiseMenuClosed(top);

        if (top.CloseAllPrevious)
        {
            CloseAllMenus();
            return;
        }

        // Reveal the next menu, but cascade any that were queued for close.
        while (menuStack.Count > 0)
        {
            var next = menuStack.Peek();

            if (pendingClose.Contains(next))
            {
                menuStack.Pop();
                pendingClose.Remove(next);
                next.SetInteractable(false);
                next.Hide();
                next.OnClosed();
                UIEvents.RaiseMenuClosed(next);
                continue;
            }

            next.OnRevealed();
            next.SetInteractable(true);
            return;
        }

        UIEvents.RaiseAllMenusClosed();
    }

    public void CloseAllMenus()
    {
        while (menuStack.Count > 0)
        {
            Menu m = menuStack.Pop();
            pendingClose.Remove(m);
            m.SetInteractable(false);
            m.Hide();
            m.OnClosed();
            UIEvents.RaiseMenuClosed(m);
        }
        pendingClose.Clear();
        UIEvents.RaiseAllMenusClosed();
    }

    #endregion

    #region Queries

    public bool IsMenuOnTop(Menu menu) => menuStack.Count > 0 && menuStack.Peek() == menu;
    public bool IsMenuOpen(Menu menu)  => menuStack.Contains(menu);

    #endregion
}
