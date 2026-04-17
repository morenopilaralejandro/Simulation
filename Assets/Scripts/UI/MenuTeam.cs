using UnityEngine;
using UnityEngine.UI;
using Simulation.Enums.Battle;

/// <summary>
/// Root menu for team loadout management.
/// Opened via MenuManager.Instance.ReplaceMenu(menuTeam).
/// Coordinates three child panels: Loadout list, Team view, and Settings.
/// </summary>
public class MenuTeam : Menu
{
    #region Fields
    private MenuManager menuManager;
    private TeamManager teamManager;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    public bool IsTeamMenuOpen => isOpen;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        menuManager = MenuManager.Instance;
        teamManager = TeamManager.Instance;
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

        UIEvents.RaiseTeamLoadoutRequested();
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
        UIEvents.OnTeamMenuClosed += HandleTeamMenuClosed;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamMenuClosed -= HandleTeamMenuClosed;
    }

    private void HandleTeamMenuClosed() 
    {
        Close();
    }
    #endregion
}
