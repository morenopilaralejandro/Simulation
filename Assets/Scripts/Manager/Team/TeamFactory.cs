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
            CustomName = TeamLoadoutManager.Instance.DEFAULT_NAME,
            CustomCrestId = TeamLoadoutManager.DEFAULT_CREST_ID,
            CustomKitId = TeamLoadoutManager.DEFAULT_KIT_ID,
            CustomFullBattleFormationId = TeamLoadoutManager.DEFAULT_FULL_BATTLE_FORMATION_ID,
            CustomFullBattleCharacterGuids = null,
            CustomMiniBattleFormationId = TeamLoadoutManager.DEFAULT_MINI_BATTLE_FORMATION_ID,
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
