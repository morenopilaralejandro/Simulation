using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Match;

public class MatchComponentAttributes
{
    public string MatchId { get; private set; }
    public string TeamId  { get; private set; }
    public int Level { get; private set; }
    public BattleType BattleType  { get; private set; }
    public string BgmId  { get; private set; }
    public string BallId  { get; private set; }
    public string FieldId  { get; private set; }

    public MatchComponentAttributes(MatchData data)
    {
        MatchId = data.MatchId;
        TeamId = data.TeamId;
        Level = data.Level;
        BattleType = data.BattleType;
        BgmId = data.BgmId;
        BallId = data.BallId;
        FieldId = data.FieldId;
    }
}
