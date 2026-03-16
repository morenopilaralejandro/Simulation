using System;
using UnityEngine;
using Simulation.Enums.Battle;

public static class UIEvents
{
    //menu
    public static event Action<Menu> OnMenuOpened;
    public static void RaiseMenuOpened(Menu menu)
    {
        OnMenuOpened?.Invoke(menu);
    }

    public static event Action<Menu> OnMenuClosed;
    public static void RaiseMenuClosed(Menu menu)
    {
        OnMenuClosed?.Invoke(menu);
    }

    public static event Action OnAllMenusClosed;
    public static void RaiseAllMenusClosed()
    {
        OnAllMenusClosed?.Invoke();
    }

    // Menu Team
    public static event Action<BattleType, BattleType> OnBattleTypeChanged;
    public static void RaiseBattleTypeChanged(BattleType newType, BattleType oldType)
    {
        OnBattleTypeChanged?.Invoke(newType, oldType);
    }

    public static event Action<Team> OnTeamLoadoutSelected;
    public static void RaiseTeamLoadoutSelected(Team team)
    {
        OnTeamLoadoutSelected?.Invoke(team);
    }

    public static event Action OnTeamLoadoutCreateRequested;
    public static void RaiseTeamLoadoutCreateRequested()
    {
        OnTeamLoadoutCreateRequested?.Invoke();
    }

    public static event Action OnTeamSettingsRequested;
    public static void RaiseTeamSettingsRequested()
    {
        OnTeamSettingsRequested?.Invoke();
    }

    public static event Action OnTeamSettingsClosed;
    public static void RaiseTeamSettingsClosed()
    {
        OnTeamSettingsClosed?.Invoke();
    }

    public static event Action OnBackFromTeamRequested;
    public static void RaiseBackFromTeamRequested()
    {
        OnBackFromTeamRequested?.Invoke();
    }

}
