using System;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public static class BattleEvents
{
    public static event Action OnAllCharactersReady;
    public static void RaiseAllCharactersReady()
    {
        OnAllCharactersReady?.Invoke();
    }

    public static event Action<BattlePhase, BattlePhase> OnBattlePhaseChanged;
    public static void RaiseBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase)
    {
        OnBattlePhaseChanged?.Invoke(newPhase, oldPhase);
    }

    public static event Action OnStartBattle;
    public static void RaiseStartBattle()
    {
        OnStartBattle?.Invoke();
    }

    public static event Action OnEndBattle;
    public static void RaiseEndBattle()
    {
        OnEndBattle?.Invoke();
    }

    public static event Action OnPauseBattle;
    public static void RaisePauseBattle()
    {
        OnPauseBattle?.Invoke();
    }

    public static event Action OnResumeBattle;
    public static void RaiseResumeBattle()
    {
        OnResumeBattle?.Invoke();
    }

}
