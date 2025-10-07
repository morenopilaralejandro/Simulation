using System.Collections.Generic;
using UnityEngine;

public class TeamComponentFormation : MonoBehaviour
{
    private Team team;

    [SerializeField] private Formation formation;

    public Formation Formation => formation;

    public void Initialize(TeamData teamData)
    {
        formation = FormationManager.Instance.GetFormation(teamData.FormationId);
    }

    public void SetTeam(Team team)
    {
        this.team = team;
    }

    public void SetFormation(Formation formation)
    {
        if (formation == null) return;
        this.formation = formation;
        TeamEvents.RaiseOnFormationChanged(team, formation);
    }
}
