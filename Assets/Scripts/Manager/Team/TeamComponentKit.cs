using System.Collections.Generic;
using UnityEngine;

public class TeamComponentKit
{
    private Team team;

    private Kit kit;

    public Kit Kit => kit;

    public TeamComponentKit(TeamData teamData, Team team) 
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        this.kit = KitManager.Instance.GetKit(teamData.KitId);
    }

    public void SetKit(Kit kit)
    {
        if (kit == null) return;
        this.kit = kit;
        TeamEvents.RaiseOnKitChanged(team, kit);
    }
}
