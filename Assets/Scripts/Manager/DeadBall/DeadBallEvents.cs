using System;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.DeadBall;

public static class DeadBallEvents
{
    public static event Action<DeadBallType, TeamSide> OnDeadBallStartRequested;
    public static void RaiseDeadBallStartRequested(DeadBallType type, TeamSide teamSide)
    {
        OnDeadBallStartRequested?.Invoke(type, teamSide);
    }

    public static event Action<DeadBallType, TeamSide, TeamSide> OnDeadBallStarted;
    public static void RaiseDeadBallStarted(DeadBallType type, TeamSide offenseSide, TeamSide defenseSide)
    {
        OnDeadBallStarted?.Invoke(type, offenseSide, defenseSide);
    }

    public static event Action<DeadBallType, TeamSide, TeamSide> OnDeadBallReady;
    public static void RaiseDeadBallReady(DeadBallType type, TeamSide offenseSide, TeamSide defenseSide)
    {
        OnDeadBallReady?.Invoke(type, offenseSide, defenseSide);
    }

    public static event Action<DeadBallType, TeamSide, TeamSide> OnDeadBallEnded;
    public static void RaiseDeadBallEnded(DeadBallType type, TeamSide offenseSide, TeamSide defenseSide)
    {
        OnDeadBallEnded?.Invoke(type, offenseSide, defenseSide);
    }
}
