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
        Battle,
        MiniBattle
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

}

