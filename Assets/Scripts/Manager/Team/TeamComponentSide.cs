using UnityEngine;
using Simulation.Enums.Character;

public class TeamComponentSide : MonoBehaviour
{
    private Team team;

    [SerializeField] private TeamSide teamSide;

    public TeamSide TeamSide => teamSide;

    public void Initialize(TeamData teamData)
    {

    }

    public void SetTeam(Team team)
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
        if (team == this.team)
        {
            this.teamSide = teamSide;
            LogManager.Trace($"[TeamComponentSide] {this.team.TeamId} assigned to side {teamSide}", this);
        }
    }
}
