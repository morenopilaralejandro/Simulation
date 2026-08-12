using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;

public static class DuelEvents
{
    public static event Action<DuelMode> OnDuelStarted;
    public static void RaiseDuelStarted(DuelMode duelMode)
    {
        OnDuelStarted?.Invoke(duelMode);
    }

    public static event Action
    <
        DuelMode,
        DuelParticipant, 
        DuelParticipant, 
        bool
    > OnDuelEnded;
    public static void RaiseDuelEnded(
        DuelMode duelMode,
        DuelParticipant winner, 
        DuelParticipant loser,
        bool isWinnerUser)
    {
        OnDuelEnded?.Invoke(duelMode, winner, loser, isWinnerUser);
    }

    public static event Action<DuelMode> OnDuelCanceled;
    public static void RaiseDuelCanceled(DuelMode duelMode)
    {
        OnDuelCanceled?.Invoke(duelMode);
    }

}
