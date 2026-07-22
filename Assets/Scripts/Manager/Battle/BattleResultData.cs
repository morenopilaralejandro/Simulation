using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Match;

[System.Serializable]
public class BattleResultData
{
    public BattleResultsType BattleResultsType { get; set; }
    public TeamSide WinningSide { get; set; }
    public Dictionary<TeamSide, int> FinalScore { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public int EnemyLv { get; set; }
    
    // Basic-rewards
    public int ExpReward { get; set; }
    public int GoldReward { get; set; }
    
    // Encounter-specific
    public List<ItemReward> ItemRewards { get; set; } //generate encounter from drops or match drops

    // Match-specific
    public MatchRank MatchRank { get; set; }
    
    public BattleResultData()
    {
        FinalScore = new Dictionary<TeamSide, int>();
        ItemRewards = new List<ItemReward>();
    }

    public void Clear() {}
}
