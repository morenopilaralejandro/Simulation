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
            TeamId = teamSaveData.TeamGuid;;
            TeamGuid = teamSaveData.TeamGuid;
        } else 
        {
            TeamId = teamData.TeamId;
            TeamGuid = Guid.NewGuid().ToString();
        }
    }
}
