using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Match;

[System.Serializable]
public class BattleResultData
{
    public BattleResultsType BattleResultsType;
    public BattleType BattleType;

    public TeamSide WinningSide;
    public TeamSide UserSide;

    public Dictionary<TeamSide, int> Scores = new();
    public Dictionary<TeamSide, Team> Teams = new();

    public int XpReward;
    public int GoldReward;
    public int EnemyLevel;

    public List<ItemReward> ItemRewards = new();
    public List<BattleResultDataXp> XpResult = new();

    public MatchRank MatchRank;

    public bool IsUserWin => !IsDraw && WinningSide == UserSide;
    public bool IsHomeWin => WinningSide == TeamSide.Home;
    public bool IsAwayWin => WinningSide == TeamSide.Away;
    public bool IsDraw => Scores[TeamSide.Home] == Scores[TeamSide.Away];

    /*
    public TeamSide LosingSide =>
        IsDraw
            ? TeamSide.None
            : (WinningSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home);
    */

    public TeamSide LosingSide => WinningSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;
    public TeamSide EnemySide => UserSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;

    public int GoalDifference => Mathf.Abs(Scores[TeamSide.Home] - Scores[TeamSide.Away]);

    public void Clear()
    {
        Scores.Clear();
        Teams.Clear();

        ItemRewards.Clear();
        XpResult.Clear();

        XpReward = 0;
        GoldReward = 0;
        EnemyLevel = 0;

        MatchRank = default;
        BattleResultsType = default;
        BattleType = default;
        WinningSide = default;
        UserSide = default;
    }
}
