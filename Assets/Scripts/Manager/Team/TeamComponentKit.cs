using System.Collections.Generic;
using UnityEngine;

public class TeamComponentKit
{
    private Team team;

    public Kit Kit { get; private set; }

    public TeamComponentKit(TeamData teamData, Team team, TeamSaveData teamSaveData = null) 
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        this.team = team;

        if (teamSaveData != null)
        {
            this.Kit = KitManager.Instance.GetKit(teamSaveData.CustomKitId);
        } else 
        {
            if (teamData != null) 
            {
                this.Kit = KitManager.Instance.GetKit(teamData.KitId);
            } else 
            {
                this.Kit = KitManager.Instance.GetKit("faith");
            }
        }

    }

    public void SetKit(Kit kit)
    {
        if (kit == null) return;
        this.Kit = kit;
        TeamEvents.RaiseKitChanged(team, kit);
    }
}
