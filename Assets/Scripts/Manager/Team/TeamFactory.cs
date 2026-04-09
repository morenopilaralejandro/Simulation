using UnityEngine;
using System;
using System.Collections.Generic;

public static class TeamFactory
{
    public static Team Create()
    {
        TeamSaveData teamSaveData = new TeamSaveData
        {
            TeamGuid = Guid.NewGuid().ToString(),
            IsCustomLoadout = true,
            CustomName = TeamManager.Instance.DEFAULT_NAME,
            CustomCrestId = TeamManager.DEFAULT_CREST_ID,
            CustomKitId = TeamManager.DEFAULT_KIT_ID,
            CustomFullBattleFormationId = TeamManager.DEFAULT_FULL_BATTLE_FORMATION_ID,
            CustomFullBattleCharacterGuids = null,
            CustomMiniBattleFormationId = TeamManager.DEFAULT_MINI_BATTLE_FORMATION_ID,
            CustomMiniBattleCharacterGuids = null
        };
        return new Team(null, teamSaveData);
    }

    public static Team Create(TeamData teamData)
    {
        return new Team(teamData);
    }

    public static Team Create(TeamSaveData teamSaveData)
    {
        return new Team(null, teamSaveData);
    }
}
