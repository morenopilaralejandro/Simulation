using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

public class TeamComponentCustomLoadout
{
    public bool IsCustomLoadout { get; private set; }
    public string CustomName { get; private set; }

    public TeamComponentCustomLoadout(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        if(teamSaveData != null)
        {
            IsCustomLoadout = teamSaveData.IsCustomLoadout;
            CustomName = teamSaveData.CustomName;
        } else 
        {
            IsCustomLoadout = false;
            CustomName = TeamLoadoutManager.Instance.DEFAULT_NAME;
        }
    }

    public void SetCustomName(string customName) => CustomName = customName;
}
