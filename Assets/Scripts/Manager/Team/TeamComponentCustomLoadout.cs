using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Localization;

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
            CustomName = TeamManager.Instance.DEFAULT_NAME;
        }
    }

    public void SetCustomName(string customName) => CustomName = customName;
}
