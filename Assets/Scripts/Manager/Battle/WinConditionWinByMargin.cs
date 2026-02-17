using System;
using System.Collections.Generic;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

/// <summary>
/// Win by a goal margin over the opponent before the timer runs out.
/// The first team to lead by the required margin wins immediately.
/// If time runs out without a margin, the leading team wins (or draw).
/// </summary>
public class WinConditionWinByMargin : WinCondition
{
    public override WinConditionType Type => WinConditionType.WinByMargin;
    public override bool HasTwoHalves => false;

    private readonly int marginRequired;

    public int MarginRequired => marginRequired;

    public WinConditionWinByMargin(int marginRequired)
    {
        this.marginRequired = marginRequired;
    }

    public override bool ShouldEndOnGoal(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf)
    {
        int diff = Math.Abs(scoreDict[TeamSide.Home] - scoreDict[TeamSide.Away]);
        return diff >= marginRequired;
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

        if (home > away) return WinConditionResult.HomeWin;
        if (away > home) return WinConditionResult.AwayWin;
        return WinConditionResult.Draw;
    }
}
