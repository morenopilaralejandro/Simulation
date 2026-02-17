using System.Collections.Generic;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

/// <summary>
/// Play two halves. At the end of the second half, the highest score wins.
/// </summary>
public class WinConditionWinMatch : WinCondition
{
    public override WinConditionType Type => WinConditionType.WinMatch;
    public override bool HasTwoHalves => true;

    public override bool ShouldEndOnGoal(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf)
    {
        // Goals never immediately end a WinMatch game
        return false;
    }

    public override bool ShouldEndOnTimeOver(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf)
    {
        // Only end after the second half
        return currentHalf == TimerHalf.Second;
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
