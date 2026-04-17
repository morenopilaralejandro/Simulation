using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

public abstract class WinCondition
{
    public abstract WinConditionType Type { get; }

    /// <summary>
    /// Whether this win condition uses two halves.
    /// </summary>
    public abstract bool HasTwoHalves { get; }

    /// <summary>
    /// Called when a goal is scored. Returns true if the game should end immediately.
    /// </summary>
    public abstract bool ShouldEndOnGoal(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf);

    /// <summary>
    /// Called when the timer expires. Returns true if the game should end.
    /// </summary>
    public abstract bool ShouldEndOnTimeOver(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf);

    /// <summary>
    /// Evaluate the final result given the current scores.
    /// </summary>
    public abstract WinConditionResult EvaluateResult(
        Dictionary<TeamSide, int> scoreDict, 
        TimerHalf currentHalf);

    /// <summary>
    /// Factory method to create a WinCondition from BattleArgs.
    /// </summary>
    public static WinCondition Create(WinConditionType type, WinConditionParams conditionParams)
    {
        switch (type)
        {
            case WinConditionType.WinMatch:
                return new WinConditionWinMatch();
            case WinConditionType.ScoreGoals:
                return new WinConditionScoreGoals(conditionParams.TargetGoals);
            case WinConditionType.WinByMargin:
                return new WinConditionWinByMargin(conditionParams.MarginRequired);
            default:
                LogManager.Warning($"[WinCondition] Unknown WinConditionType: {type}. Falling back to WinMatch.");
                return new WinConditionWinMatch();
        }
    }
}
