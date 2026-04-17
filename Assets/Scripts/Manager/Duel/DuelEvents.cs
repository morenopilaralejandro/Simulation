using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;

public static class DuelEvents
{
    public static event Action<DuelMode> OnDuelStart;
    public static void RaiseDuelStart(DuelMode duelMode)
    {
        OnDuelStart?.Invoke(duelMode);
    }

    public static event Action
    <
        DuelMode,
        DuelParticipant, 
        DuelParticipant, 
        bool
    > OnDuelEnd;
    public static void RaiseDuelEnd(
        DuelMode duelMode,
        DuelParticipant winner, 
        DuelParticipant loser,
        bool isWinnerUser)
    {
        OnDuelEnd?.Invoke(duelMode, winner, loser, isWinnerUser);
    }

    public static event Action<DuelMode> OnDuelCancel;
    public static void RaiseDuelCancel(DuelMode duelMode)
    {
        OnDuelCancel?.Invoke(duelMode);
    }

}
