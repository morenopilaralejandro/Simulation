using System.Collections.Generic;
using UnityEngine;

public class TeamComponentAttributes
{
    public string TeamId { get; private set; }

    public TeamComponentAttributes(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.TeamId = teamData.TeamId;
    }
}
