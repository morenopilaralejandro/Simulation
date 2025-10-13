using System.Collections.Generic;
using UnityEngine;

public class TeamComponentFormation
{
    private Team team;

    public Formation Formation { get; private set; }

    public TeamComponentFormation(TeamData teamData, Team team) 
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        this.Formation = FormationManager.Instance.GetFormation(teamData.FormationId);
    }

    public void SetFormation(Formation formation)
    {
        if (formation == null) return;
        this.Formation = formation;
        TeamEvents.RaiseOnFormationChanged(team, formation);
    }
}
