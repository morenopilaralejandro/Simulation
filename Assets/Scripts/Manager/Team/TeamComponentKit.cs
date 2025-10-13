using System.Collections.Generic;
using UnityEngine;

public class TeamComponentKit
{
    private Team team;

    public Kit Kit { get; private set; }

    public TeamComponentKit(TeamData teamData, Team team) 
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        this.Kit = KitManager.Instance.GetKit(teamData.KitId);
    }

    public void SetKit(Kit kit)
    {
        if (kit == null) return;
        this.Kit = kit;
        TeamEvents.RaiseOnKitChanged(team, kit);
    }
}
