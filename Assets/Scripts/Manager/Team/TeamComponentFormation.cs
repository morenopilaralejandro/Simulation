using System.Collections.Generic;
using UnityEngine;

public class TeamComponentFormation
{
    private Team team;

    [SerializeField] private Formation formation;

    public Formation Formation => formation;

    public TeamComponentFormation(TeamData teamData, Team team) 
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        this.formation = FormationManager.Instance.GetFormation(teamData.FormationId);
    }

    public void SetFormation(Formation formation)
    {
        if (formation == null) return;
        this.formation = formation;
        TeamEvents.RaiseOnFormationChanged(team, formation);
    }
}
