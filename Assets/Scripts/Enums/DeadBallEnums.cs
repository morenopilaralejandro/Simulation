namespace Simulation.Enums.DeadBall
{
    public enum DeadBallState
    {
        None,
        Setup,
        WaitingForReady,
        Executing,
        Finished
    }

    public enum DeadBallType
    {
        Kickoff,
        ThrowIn,
        CornerKick,
        GoalKick,
        FreeKickIndirect,
        Penalty
    }

}

