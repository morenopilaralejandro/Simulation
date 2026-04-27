using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;

/// <summary>

/// </summary>
public class MenuCharacter : Menu
{
    #region Fields
    private MenuManager menuManager;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    public bool IsCharacterMenuOpen => isOpen;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        menuManager = MenuManager.Instance;
    }

    private void Start()
    {
        base.Hide();
        base.SetInteractable(false);
    }

    /*
    private void Update()
    {
        HandleInput();
    }
    */

    #endregion

    #region Menu Overrides

    /// <summary>
    /// Called externally by MenuManager.Instance.ReplaceMenu(menuTeam).
    /// </summary>
    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        UIEvents.RaiseCharacterSelectorOpened(null, default);
    }

    public override void Hide()
    {
        base.SetInteractable(false);
        base.Hide();
    }

    #endregion

    #region Navigation

    public void Close()
    {
        if (!isOpen) return;
        menuManager.CloseMenu();
    }

    #endregion

    #region Input

    private void HandleInput()
    {
        // Placeholder for input handling
        /*
        if (isOpen)
        {
            if (InputManager.Instance.GetDown(CustomAction.World_CloseSideMenu))
                Close();
        }
        else
        {
            if (!WorldManager.Instance.PlayerCanOpenMenu) return;
            if (InputManager.Instance.GetDown(CustomAction.World_OpenSideMenu))
                Open();
        }
        */
    }

    #endregion

    #region Button Handle

    /*
    public void OnButtonCloseTapped() 
    {
        Close();
    }
    */

    #endregion

    #region Event

    private void OnEnable()
    {
        UIEvents.OnBackFromCharacterSelectorRequested += HandleBackFromCharacterSelectorRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnBackFromCharacterSelectorRequested -= HandleBackFromCharacterSelectorRequested;
    }

    private void HandleBackFromCharacterSelectorRequested() 
    {
        if (!isTop) return;
        Close();
    }
    #endregion
}
