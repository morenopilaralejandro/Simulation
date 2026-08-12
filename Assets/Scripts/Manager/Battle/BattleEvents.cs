using System;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;

public static class BattleEvents
{
    public static event Action OnAllCharactersReady;
    public static void RaiseAllCharactersReady()
    {
        OnAllCharactersReady?.Invoke();
    }

    public static event Action<BattlePhase, BattlePhase> OnBattlePhaseChangeRequested;
    public static void RaiseBattlePhaseChangeRequested(BattlePhase newPhase, BattlePhase oldPhase)
    {
        OnBattlePhaseChangeRequested?.Invoke(newPhase, oldPhase);
    }

    public static event Action<BattlePhase, BattlePhase> OnBattlePhaseChanged;
    public static void RaiseBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase)
    {
        OnBattlePhaseChanged?.Invoke(newPhase, oldPhase);
    }

    public static event Action<BattleType, BattleType> OnBattleTypeChanged;
    public static void RaiseBattleTypeChanged(BattleType newType, BattleType oldType)
    {
        OnBattleTypeChanged?.Invoke(newType, oldType);
    }

    public static event Action OnBattleStartRequested;
    public static void RaiseBattleStartRequested()
    {
        OnBattleStartRequested?.Invoke();
    }

    public static event Action<BattleType> OnBattleStarted;
    public static void RaiseBattleStarted(BattleType battleType)
    {
        OnBattleStarted?.Invoke(battleType);
        InputEvents.RaiseScreenControlsShowRequested();
        InputEvents.RaiseDirectionalInputModeChanged(DirectionalInputMode.Joystick);
    }

    public static event Action OnBattleEnded;
    public static void RaiseBattleEnded()
    {
        OnBattleEnded?.Invoke();
        InputEvents.RaiseScreenControlsHideRequested();
        InputEvents.RaiseDirectionalInputModeChanged(DirectionalInputMode.Dpad);
    }

    public static event Action OnBattleFreezeRequested;
    public static void RaiseBattleFreezeRequested()
    {
        OnBattleFreezeRequested?.Invoke();
    }

    public static event Action OnBattleFroze;
    public static void RaiseBattleFroze()
    {
        OnBattleFroze?.Invoke();
    }

    public static event Action OnBattleUnfreezeRequested;
    public static void RaiseBattleUnfreezeRequested()
    {
        OnBattleUnfreezeRequested?.Invoke();
    }

    public static event Action OnBattleUnfroze;
    public static void RaiseBattleUnfroze()
    {
        OnBattleUnfroze?.Invoke();
    }

    public static event Action<TeamSide> OnBattlePauseRequested;
    public static void RaiseBattlePauseRequested(TeamSide teamSide)
    {
        OnBattlePauseRequested?.Invoke(teamSide);
    }

    public static event Action<TeamSide> OnBattlePaused;
    public static void RaiseBattlePaused(TeamSide teamSide)
    {
        OnBattlePaused?.Invoke(teamSide);
    }

    public static event Action OnBattleResumed;
    public static void RaiseBattleResumed()
    {
        OnBattleResumed?.Invoke();
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

    public static event Action OnResultsContinueRequested;
    public static void RaiseResultsContinueRequested()
    {
        OnResultsContinueRequested?.Invoke();
    }

}
