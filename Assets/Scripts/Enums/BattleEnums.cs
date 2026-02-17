namespace Simulation.Enums.Battle
{
    public enum BattlePhase
    {
        Battle,
        Selection,
        Cutscene,
        DeadBall,
        End
    }

    public enum BattleType
    {
        Full,
        Mini
    }

    public enum BoundPlacement
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum CornerPlacement
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public enum BoundType
    {
        Wallbounce,
        GoalPost,
        GoalNet,
        Endline,
        Sideline,
        Corner,
    }

    public enum TimerHalf 
    {
        First,
        Second
    }

    public enum MessageType
    {
        Goal,
        HalfTime,
        FullTime,
        TimeUp,
        Foul,
        Offside
    }

    public enum WinConditionType
    {
        WinMatch,
        ScoreGoals,
        WinByMargin
    }

    public enum WinConditionResult
    {
        Undecided,      // Game still in progress
        HomeWin,
        AwayWin,
        Draw
    }

}

