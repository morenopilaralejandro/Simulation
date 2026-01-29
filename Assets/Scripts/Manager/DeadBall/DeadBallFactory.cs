using Simulation.Enums.DeadBall;

public static class DeadBallFactory
{
    public static IDeadBallHandler Create(DeadBallType type)
    {
        switch (type)
        {
            case DeadBallType.Kickoff:
                return new DeadBallKickoffHandler();

            case DeadBallType.ThrowIn:
                return new DeadBallThrowInHandler();

            case DeadBallType.CornerKick:
                return new DeadBallCornerKickHandler();

            case DeadBallType.FreeKickIndirect:
                return new DeadBallFreeKickIndirectHandler();

            case DeadBallType.GoalKick:
                return new DeadBallGoalKickHandler();

/*

            case DeadBallType.Penalty:
                return new DeadBallPenaltyHandler();
*/
            default:
                LogManager.Error($"DeadBallType {type} not supported");
                return null;

        }
    }
}
