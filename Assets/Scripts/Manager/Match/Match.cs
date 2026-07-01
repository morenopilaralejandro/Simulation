using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Match;

public class Match
{
    #region Components

    private MatchComponentAttributes attributesComponent;

    #endregion

    #region Initialize

    public Match(MatchData data) 
    {
        attributesComponent = new MatchComponentAttributes(data);
    }

    #endregion

    #region API

    // attributesComponent
    public string MatchId => attributesComponent.MatchId;
    public string TeamId => attributesComponent.TeamId;
    public int Level => attributesComponent.Level;
    public BattleType BattleType => attributesComponent.BattleType;
    public string BgmId => attributesComponent.BgmId;
    public string BallId => attributesComponent.BallId;
    public string FieldId => attributesComponent.FieldId;

    #endregion
}
