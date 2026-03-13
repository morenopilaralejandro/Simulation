using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamComponentAttributes
{
    public string TeamId { get; private set; }
    public string TeamGuid { get; private set; }

    public TeamComponentAttributes(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        if(teamSaveData != null)
        {
            TeamGuid = teamSaveData.TeamGuid;
            TeamId = teamSaveData.TeamGuid;
        } else 
        {
            TeamGuid = Guid.NewGuid().ToString();
            if (teamData != null) 
                TeamId = teamData.TeamId;
            else 
                TeamId = TeamGuid;
        }
    }
}
