using System.Collections.Generic;
using UnityEngine;

public class TeamComponentAttributes
{
    private string teamId;

    public string TeamId => teamId;

    public TeamComponentAttributes(TeamData teamData)
    {
        Initialize(teamData);
    }

    public void Initialize(TeamData teamData)
    {
        this.teamId = teamData.TeamId;
    }
}
