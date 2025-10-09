using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;

public class TeamComponentSide
{
    private Team team;

    private TeamSide teamSide;
    private Variant variant;

    public TeamSide TeamSide => teamSide;
    public Variant Variant => variant;

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
        TeamEvents.OnAssignVariantToTeam += HandleAssignVariantToTeam;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignTeamToSide -= HandleAssignTeamToSide;
        TeamEvents.OnAssignVariantToTeam -= HandleAssignVariantToTeam;
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

    private void HandleAssignVariantToTeam(
        Team team, 
        Variant variant)
    {
        if (this.team == team)
        {
            this.variant = variant;
            LogManager.Trace($"[TeamComponentSide] {this.team.TeamId} assigned to variant {variant}", null);
        }
    }
}
