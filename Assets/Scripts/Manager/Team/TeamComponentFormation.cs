using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

public class TeamComponentFormation
{
    private Team team;

    public Formation FullBattleFormation { get; private set; }
    public Formation MiniBattleFormation { get; private set; }

    public TeamComponentFormation(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        this.team = team;

        if (teamSaveData != null)
        {
            FullBattleFormation = FormationManager.Instance.GetFormation(teamSaveData.CustomFullBattleFormationId);
            MiniBattleFormation = FormationManager.Instance.GetFormation(teamSaveData.CustomMiniBattleFormationId);
        } else 
        {
            if (teamData != null) 
            {
                FullBattleFormation = FormationManager.Instance.GetFormation(teamData.FullBattleFormationId);
                MiniBattleFormation = FormationManager.Instance.GetFormation(teamData.MiniBattleFormationId);
            } else 
            {
                FullBattleFormation = FormationManager.Instance.GetFormation("crimson");
                MiniBattleFormation = FormationManager.Instance.GetFormation("offense");
            }
        }
    }

    public Formation GetFormation(BattleType battleType)
    {
        return battleType switch
        {
            BattleType.Full => FullBattleFormation,
            BattleType.Mini => MiniBattleFormation,
            _ => FullBattleFormation
        };
    }

    public void SetFormation(Formation formation, BattleType battleType)
    {
        if (formation == null) return;
        switch (battleType)
        {
            case BattleType.Full:
                FullBattleFormation = formation;
                break;
            case BattleType.Mini:
                MiniBattleFormation = formation;
                break;
            default:
                LogManager.Warning($"[TeamComponentFormation] Unknown battle type: {battleType}");
                return;
        }
        TeamEvents.RaiseFormationChanged(team, formation);
    }
}
