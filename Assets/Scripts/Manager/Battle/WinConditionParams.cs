using System;

[Serializable]
public class WinConditionParams
{
    /// <summary>
    /// Used by ScoreGoals: how many goals a team must score to win.
    /// </summary>
    public int TargetGoals;

    /// <summary>
    /// Used by WinByMargin: the goal difference required to win.
    /// </summary>
    public int MarginRequired;

    public WinConditionParams()
    {
        TargetGoals = 1;
        MarginRequired = 2;
    }

    public WinConditionParams(int targetGoals = 1, int marginRequired = 2)
    {
        TargetGoals = targetGoals;
        MarginRequired = marginRequired;
    }
}
