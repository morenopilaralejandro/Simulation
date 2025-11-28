using System;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;

public static class DuelEvents
{
    public static event Action<DuelMode> OnDuelStart;
    public static void RaiseDuelStart(DuelMode duelMode)
    {
        OnDuelStart?.Invoke(duelMode);
    }

    public static event Action<DuelParticipant, DuelParticipant, bool> OnDuelEnd;
    public static void RaiseDuelEnd(
        DuelParticipant winner, 
        DuelParticipant loser,
        bool isWinnerUser)
    {
        OnDuelEnd?.Invoke(winner, loser, isWinnerUser);
    }

    public static event Action OnDuelCancel;
    public static void RaiseDuelCancel()
    {
        OnDuelCancel?.Invoke();
    }

}
