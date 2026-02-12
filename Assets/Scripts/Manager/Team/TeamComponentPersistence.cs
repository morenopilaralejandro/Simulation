using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class TeamComponentPersistence
{
    #region Fields

    private Team team;

    #endregion        

    #region LifeCycle

    public TeamComponentPersistence(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
    }

    #endregion

    #region Import

    public void Import(TeamSaveData teamSaveData)
    {
        team.Initialize(null, teamSaveData);
    }

    #endregion

    #region Export

    public TeamSaveData Export()
    {
        return new TeamSaveData
        {
            TeamGuid = team.TeamGuid,
            IsCustomLoadout = team.IsCustomLoadout,
            CustomName = team.CustomName,
            CustomCrestId = team.CustomCrestId,
            CustomKitId = team.CustomKitId,
            CustomFullBattleFormationId = team.CustomFullBattleFormationId,
            CustomFullBattleCharacterGuids = null,
            CustomMiniBattleFormationId = team.CustomMiniBattleFormationId,
            CustomMiniBattleCharacterGuids = null
        };
    }

    #endregion

    #region Helpers

    #endregion
}
