using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Match;
using Aremoreno.Enums.World;

[CreateAssetMenu(fileName = "MatchData", menuName = "ScriptableObject/Match/MatchData")]
public class MatchData : ScriptableObject
{
    public string MatchId;
    public string TeamId;
    public int Level;
    public BattleType BattleType;
    public string BgmId;
    public string BallId;
    public string FieldId;
    public bool HasTimeOfDayRestriction;
    public TimeOfDay TimeOfDay;
}
