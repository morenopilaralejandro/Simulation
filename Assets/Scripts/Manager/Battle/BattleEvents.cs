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

    public static event Action OnBattleStart;
    public static void RaiseBattleStart()
    {
        OnBattleStart?.Invoke();
    }

    public static event Action OnBattleEnd;
    public static void RaiseBattleEnd()
    {
        OnBattleEnd?.Invoke();
    }

    public static event Action OnFreeze;
    public static void RaiseFreeze()
    {
        OnFreeze?.Invoke();
    }

    public static event Action OnUnfreeze;
    public static void RaiseUnfreeze()
    {
        OnUnfreeze?.Invoke();
    }

    public static event Action<TeamSide> OnBattlePause;
    public static void RaiseBattlePause(TeamSide teamSide)
    {
        OnBattlePause?.Invoke(teamSide);
    }

    public static event Action OnBattleResume;
    public static void RaiseBattleResume()
    {
        OnBattleResume?.Invoke();
    }

    public static event Action<CharacterEntityBattle> OnGoalScored;
    public static void RaiseGoalScored(CharacterEntityBattle scorringCharacter)
    {
        OnGoalScored?.Invoke(scorringCharacter);
    }

    public static event Action<CharacterEntityBattle> OnPassPerformed;
    public static void RaisePassPerformed(CharacterEntityBattle character)
    {
        OnPassPerformed?.Invoke(character);
    }

    public static event Action<CharacterEntityBattle, bool> OnShootPerformed;
    public static void RaiseShootPerformed(CharacterEntityBattle character, bool isDirect)
    {
        OnShootPerformed?.Invoke(character, isDirect);
    }

    public static event Action<CharacterEntityBattle> OnShootStopped;
    public static void RaiseShootStopped(CharacterEntityBattle character)
    {
        OnShootStopped?.Invoke(character);
    }

}
