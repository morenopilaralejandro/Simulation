using System.Collections.Generic;
using UnityEngine;

public class TeamComponentKit
{
    private Team team;

    public Kit Kit { get; private set; }

    public TeamComponentKit(TeamData teamData, Team team, TeamSaveData teamSaveData = null) 
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        this.team = team;

        if (teamSaveData != null)
        {
            this.Kit = KitManager.Instance.GetKit(teamSaveData.CustomKitId);
        } else 
        {
            this.Kit = KitManager.Instance.GetKit(teamData.KitId);
        }

    }

    public void SetKit(Kit kit)
    {
        if (kit == null) return;
        this.Kit = kit;
        TeamEvents.RaiseKitChanged(team, kit);
    }
}
