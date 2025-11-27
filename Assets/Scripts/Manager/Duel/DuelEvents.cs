using System;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;

public static class DuelEvents
{
    public static event Action OnDuelStart;
    public static void RaiseDuelStart()
    {
        OnDuelStart?.Invoke();
    }

    public static event Action OnDuelEnd;
    public static void RaiseDuelEnd()
    {
        OnDuelEnd?.Invoke();
    }

    public static event Action OnDuelCancel;
    public static void RaiseDuelCancel()
    {
        OnDuelCancel?.Invoke();
    }

}
