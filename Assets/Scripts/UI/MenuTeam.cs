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
    //TODO support edit or view mode
    [Header("References")]
    [SerializeField] private MenuTeamPanelLoadout panelLoadout;
    [SerializeField] private MenuTeamPanelTeam panelTeam;
    [SerializeField] private MenuTeamPanelSettings panelSettings;

    private MenuManager menuManager;
    private TeamLoadoutManager loadoutManager;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    public bool IsTeamMenuOpen => isOpen;

    // Currently inspected loadout + battle type
    private Team selectedLoadout;
    private BattleType currentBattleType = BattleType.Full;

    public Team SelectedLoadout => selectedLoadout;
    public BattleType CurrentBattleType => currentBattleType;

    #region Lifecycle

    private void Awake()
    {
        menuManager = MenuManager.Instance;
        loadoutManager = TeamLoadoutManager.Instance;
    }

    private void Start()
    {
        base.Hide();
        base.SetInteractable(false);

        HideAllPanels();
    }

    private void OnEnable()
    {
        UIEvents.OnTeamLoadoutSelected += HandleLoadoutSelected;
        UIEvents.OnTeamLoadoutCreateRequested += HandleCreateRequested;

        UIEvents.OnBackFromTeamRequested += HandleBackToLoadoutList;
        UIEvents.OnTeamSettingsRequested += HandleOpenSettings;
        UIEvents.OnBattleTypeChanged += HandleBattleTypeChanged;

        UIEvents.OnTeamSettingsClosed += HandleSettingsClosed;

        TeamEvents.OnLoadoutDeleted += HandleLoadoutDeleted;
        TeamEvents.OnLoadoutUpdated += HandleLoadoutUpdatedExternally;
    }

    private void OnDisable()
    {
        UIEvents.OnTeamLoadoutSelected -= HandleLoadoutSelected;
        UIEvents.OnTeamLoadoutCreateRequested -= HandleCreateRequested;

        UIEvents.OnBackFromTeamRequested -= HandleBackToLoadoutList;
        UIEvents.OnTeamSettingsRequested -= HandleOpenSettings;
        UIEvents.OnBattleTypeChanged -= HandleBattleTypeChanged;

        UIEvents.OnTeamSettingsClosed -= HandleSettingsClosed;

        TeamEvents.OnLoadoutDeleted -= HandleLoadoutDeleted;
        TeamEvents.OnLoadoutUpdated -= HandleLoadoutUpdatedExternally;
    }

    private void Update()
    {
        HandleInput();
    }

    #endregion

    #region Menu Overrides

    /// <summary>
    /// Called externally by MenuManager.Instance.ReplaceMenu(menuTeam).
    /// </summary>
    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        selectedLoadout = null;
        currentBattleType = BattleType.Full;

        ShowLoadoutPanel();
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
    }

    public override void Hide()
    {
        HideAllPanels();
        selectedLoadout = null;

        AudioManager.Instance.PlaySfx("sfx-menu_tap");

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

    private void ShowLoadoutPanel()
    {
        HideAllPanels();
        panelLoadout.Show();
    }

    private void ShowTeamPanel(Team team)
    {
        HideAllPanels();
        selectedLoadout = team;
        panelTeam.Show(team, currentBattleType);
    }

    private void ShowSettingsPanel(Team team)
    {
        // Settings overlays on top of team panel — don't hide team panel
        panelSettings.Show(team);
    }

    private void HideAllPanels()
    {
        panelLoadout.Hide();
        panelTeam.Hide();
        panelSettings.Hide();
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

    public void OnButtonCloseTapped() 
    {
        Close();
    }

    #endregion

    #region Event Handlers

    private void HandleLoadoutSelected(Team team)
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        ShowTeamPanel(team);
    }

    private void HandleCreateRequested()
    {
        Team newLoadout = loadoutManager.CreateLoadout();
        if (newLoadout != null)
        {
            AudioManager.Instance.PlaySfx("sfx-menu_tap");
            // Refresh the list, then navigate into the new loadout
            panelLoadout.Refresh();
            ShowTeamPanel(newLoadout);
        }
    }

    private void HandleBackToLoadoutList()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        selectedLoadout = null;
        ShowLoadoutPanel();
    }

    private void HandleOpenSettings()
    {
        if (selectedLoadout == null) return;
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        ShowSettingsPanel(selectedLoadout);
    }

    private void HandleBattleTypeChanged(BattleType newType, BattleType oldType)
    {
        currentBattleType = newType;
        AudioManager.Instance.PlaySfx("sfx-menu_tap");

        if (selectedLoadout != null)
            panelTeam.Show(selectedLoadout, currentBattleType);
    }

    private void HandleSettingsClosed()
    {
        panelSettings.Hide();
        // Refresh the team panel in case name changed
        if (selectedLoadout != null)
            panelTeam.Refresh(selectedLoadout, currentBattleType);
    }

    private void HandleLoadoutDeleted(Team deletedTeam)
    {
        panelSettings.Hide();
        selectedLoadout = null;
        ShowLoadoutPanel();
    }

    private void HandleLoadoutUpdatedExternally(Team team)
    {
        // If the currently viewed team was updated, refresh
        if (selectedLoadout != null && selectedLoadout.TeamGuid == team.TeamGuid)
        {
            panelTeam.Refresh(team, currentBattleType);
        }
    }

    #endregion
}
