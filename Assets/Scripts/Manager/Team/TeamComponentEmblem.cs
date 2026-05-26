using System.Collections.Generic;
using UnityEngine;

public class TeamComponentEmblem
{
    private Team team;

    public Emblem Emblem { get; private set; }

    public TeamComponentEmblem(TeamData teamData, Team team, TeamSaveData teamSaveData = null) 
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        this.team = team;

        if (teamSaveData != null)
        {
            this.Emblem = EmblemDatabase.Instance.GetEmblem(teamSaveData.CustomEmblemId);
        } else 
        {
            this.Emblem = EmblemDatabase.Instance.GetEmblem(teamData.EmblemId);
        }

        /*
        Get from EmblemDatabase by id

        if (teamSaveData != null) 
        {
            TeamCrestId = teamSaveData.CustomCrestId;
            _ = InitializeAsync(TeamCrestId);
            return;
        }


        if (BattleArgs.Context == RandomEncounter) 
        {
            auxId = isRare ? TeamLoadoutManager.TEAM_CREST_ID_RARE : TeamLoadoutManager.TEAM_CREST_ID_COMMON
        } else 
        {
            auxId = teamData.TeamId;
        }

        this.Emblem = EmblemDatabase.Instance.GetEmblem(auxId);

        */

    }

    public void SetEmblem(Emblem emblem)
    {
        if (emblem == null) return;
        this.Emblem = emblem;
        //TeamEvents.RaiseEmblemChanged(team, emblem);
    }
}
