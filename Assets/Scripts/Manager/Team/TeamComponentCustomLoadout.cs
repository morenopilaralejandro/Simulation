using System.Collections.Generic;
using UnityEngine;

public class TeamComponentCustomLoadout
{

    public bool IsCustomLoadout { get; set; }
    public string CustomName { get; set; }
    public string CustomCrestId { get; set; }
    public string CustomKitId { get; set; }
    public string CustomFullBattleFormationId { get; set; }
    public List<string> CustomFullBattleCharacterGuids { get; set; }
    public string CustomMiniBattleFormationId { get; set; }
    public List<string> CustomMiniBattleCharacterGuids { get; set; }

    private string defaultName = "custom team";
    private string defaultCrestId = "faith";
    private string defaultKitId = "faith";
    public string defaultBattleFormationId = "faith";
    public string defaultMiniBattleFormationId = "faith";

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
            CustomCrestId = teamSaveData.CustomCrestId;
            CustomKitId = teamSaveData.CustomKitId;
            CustomFullBattleFormationId = teamSaveData.CustomFullBattleFormationId;
            CustomFullBattleCharacterGuids = teamSaveData.CustomFullBattleCharacterGuids;
            CustomMiniBattleFormationId = teamSaveData.CustomMiniBattleFormationId;
            CustomMiniBattleCharacterGuids = teamSaveData.CustomMiniBattleCharacterGuids;
        } else 
        {
            if (teamData != null)
                IsCustomLoadout = true;
            else 
                IsCustomLoadout = false;

            CustomName = defaultName;
            CustomCrestId = defaultCrestId;
            CustomKitId = defaultKitId;
            CustomFullBattleFormationId = defaultBattleFormationId;
            CustomFullBattleCharacterGuids = new List<string>();
            CustomMiniBattleFormationId = defaultMiniBattleFormationId;
            CustomMiniBattleCharacterGuids = new List<string>();
        }
    }
}
