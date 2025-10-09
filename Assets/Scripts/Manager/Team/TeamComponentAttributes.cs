using System.Collections.Generic;
using UnityEngine;

public class TeamComponentAttributes
{
    private string teamId;

    public string TeamId => teamId;

    public TeamComponentAttributes(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.teamId = teamData.TeamId;
    }
}
