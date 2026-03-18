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
            CustomCrestId = team.TeamCrestId,
            CustomKitId = team.Kit.KitId,
            CustomFullBattleFormationId = team.FullBattleFormation.FormationId,
            CustomFullBattleCharacterGuids = team.FullBattleCharacterGuids,
            CustomMiniBattleFormationId = team.MiniBattleFormation.FormationId,
            CustomMiniBattleCharacterGuids = team.MiniBattleCharacterGuids
        };
    }

    #endregion

    #region Helpers

    #endregion
}
