using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

/// <summary>
/// Score a set amount of goals before the timer runs out.
/// The first team to reach the target wins immediately.
/// If time runs out, the team closer to the target (or with more goals) wins.
/// </summary>
public class WinConditionScoreGoals : WinCondition
{
    public override WinConditionType Type => WinConditionType.ScoreGoals;
    public override bool HasTwoHalves => false;

    private readonly int targetGoals;

    public int TargetGoals => targetGoals;

    public WinConditionScoreGoals(int targetGoals)
    {
        this.targetGoals = targetGoals;
    }

    public override bool ShouldEndOnGoal(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf)
    {
        // End immediately if either team reaches the target
        return scoreDict[TeamSide.Home] >= targetGoals || 
               scoreDict[TeamSide.Away] >= targetGoals;
    }

    public override bool ShouldEndOnTimeOver(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf)
    {
        // Time ran out — game ends regardless
        return true;
    }

    public override WinConditionResult EvaluateResult(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf)
    {
        int home = scoreDict[TeamSide.Home];
        int away = scoreDict[TeamSide.Away];

        // If someone hit the target, they win
        if (home >= targetGoals && away >= targetGoals)
        {
            // Both hit target simultaneously (unlikely but handle it)
            return home > away ? WinConditionResult.HomeWin :
                   away > home ? WinConditionResult.AwayWin :
                   WinConditionResult.Draw;
        }
        if (home >= targetGoals) return WinConditionResult.HomeWin;
        if (away >= targetGoals) return WinConditionResult.AwayWin;

        // Time ran out — nobody reached target
        if (home > away) return WinConditionResult.HomeWin;
        if (away > home) return WinConditionResult.AwayWin;
        return WinConditionResult.Draw;
    }
}
