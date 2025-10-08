using UnityEngine;
using Simulation.Enums.Character;

public class TeamComponentSide
{
    private Team team;

    private TeamSide teamSide;

    public TeamSide TeamSide => teamSide;

    public TeamComponentSide(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignTeamToSide += HandleAssignTeamToSide;    
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignTeamToSide -= HandleAssignTeamToSide;
    }

    private void HandleAssignTeamToSide(
        Team team, 
        TeamSide teamSide)
    {
        if (this.team == team)
        {
            this.teamSide = teamSide;
            LogManager.Trace($"[TeamComponentSide] {this.team.TeamId} assigned to side {teamSide}", null);
        }
    }
}
