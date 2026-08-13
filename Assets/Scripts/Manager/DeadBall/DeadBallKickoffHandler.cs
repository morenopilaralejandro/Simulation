using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.DeadBall;
using Aremoreno.Enums.Input;

public class DeadBallKickoffHandler : IDeadBallHandler
{
    #region Fields

    private Team team;
    private DeadBallManager deadBallManager;
    private CharacterEntityBattle characterKicker;
    private CharacterEntityBattle characterReceiver;
    private CharacterEntityBattle characterPassTargetAi;


    private bool isKickExecuted;
    private bool isBallReady;
    private bool isAutoBattleEnabled;
    private bool isMultiplayer;
    private Coroutine ballReadyRoutine;

    public bool IsReady => isBallReady && isKickExecuted;

    #endregion

    #region Interface

    public void Setup(TeamSide teamSide)
    {
        deadBallManager = DeadBallManager.Instance;
        isAutoBattleEnabled = AutoBattleManager.Instance.IsAutoBattleEnabled;
        isMultiplayer = false;
        isBallReady = false;

        team = BattleManager.Instance.Teams[teamSide];

        ResetPositions();

        DuelLogManager.Instance.AddDeadBallKickoff(characterKicker.Character, characterKicker.TeamSide);
    }

    public void ResetPositions()
    {

        UnsubscribeInputPass();

        isKickExecuted = false;
        isBallReady = false;
        BallEvents.OnGained -= OnBallGained;
        deadBallManager.StopRoutine(ballReadyRoutine);

        characterKicker = deadBallManager.CharacterSelector.GetKickoffKicker(team);
        characterReceiver = deadBallManager.CharacterSelector.GetKickoffReceiver(team, characterKicker);

        if (team.TeamSide == BattleManager.Instance.GetUserSide())
            CharacterChangeControlManager.Instance.SetControlledCharacter(characterKicker, team.TeamSide);

        LogManager.Trace($"[DeadBallKickoffHandler] Kickoff - Kicker: {characterKicker.name}, Receiver: {characterReceiver.name}, Same: {characterKicker == characterReceiver}");

        SetPositions();

        characterPassTargetAi = deadBallManager.CharacterSelector.GetPassTargetAi(team, characterKicker);

        BallEvents.OnGained += OnBallGained;
    }

    private void OnBallGained(CharacterEntityBattle c)
    {
        if (c == characterKicker) 
        {
            BallEvents.OnGained -= OnBallGained;
            ballReadyRoutine = deadBallManager.StartRoutine(DelayedBallReady());
        } else 
        {
            c.KickBallTo(characterKicker.transform.position);
        }
    }

    private IEnumerator DelayedBallReady()
    {
        yield return null;
        isBallReady = true;
        deadBallManager.SetState(DeadBallState.WaitingForReady);

        SubscribeInputConfirm();

        bool needsOffenseInput =
            deadBallManager.IsUserOffense &&
            !characterKicker.IsEnemyAI &&
            !isAutoBattleEnabled;

        if (needsOffenseInput)
            SubscribeInputPass();

        bool aiKicker = characterKicker.IsEnemyAI
            && characterKicker.TeamSide == deadBallManager.OffenseSide;

        if (aiKicker)
            isKickExecuted = true;

        if (isAutoBattleEnabled)
        {
            if (deadBallManager.IsUserOffense)
                isKickExecuted = true;

            deadBallManager.TeamReadiness.SetBothReady();
            deadBallManager.NotifyReadinessChanged();
        }
    }

    public void Execute()
    {
        UnsubscribeInputConfirm();
        UnsubscribeInputPass();

        CharacterEntityBattle target = BattleManager.Instance.TargetedCharacter[characterKicker.TeamSide];

        if (!target || target == characterKicker || characterKicker.IsEnemyAI || isAutoBattleEnabled) 
        {
            if (characterKicker.IsEnemyAI)
                target = characterPassTargetAi;
            else
                target = characterReceiver;

            characterKicker.KickBallTo(target.transform.position);

            if (isAutoBattleEnabled)
                CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
            else if (characterKicker.IsEnemyAI)
                CharacterChangeControlManager.Instance.TryChangeOnDeadBallGeneric(target);
        }
        else 
        {
            characterKicker.KickBallTo(target.transform.position);
            CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
        }
    }

    private void MarkKickExecuted()
    {
        if (isKickExecuted) return;
        isKickExecuted = true;
        UnsubscribeInputPass();
        deadBallManager.NotifyHandlerReady();
    }

    #endregion

    #region Input

    private void SubscribeInputConfirm()
    {
        InputManager.Instance.SubscribeDown(CustomAction.BattleUI_DeadBallConfirm, HandleConfirmPressed);
    }

    private void UnsubscribeInputConfirm()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.BattleUI_DeadBallConfirm, HandleConfirmPressed);
    }

    private void SubscribeInputPass()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Battle_Pass, HandlePassPressed);
    }

    private void UnsubscribeInputPass()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Battle_Pass, HandlePassPressed);
    }

    private void HandleConfirmPressed()
    {
        if (deadBallManager.DeadBallState != DeadBallState.WaitingForReady) return;
        if (deadBallManager.IsUserMenuOpen()) return;

        if (isMultiplayer)
            deadBallManager.TeamReadiness.SetUserReady();
        else
            deadBallManager.TeamReadiness.SetBothReady();

        deadBallManager.NotifyReadinessChanged();

        if (deadBallManager.DeadBallState == DeadBallState.Executing)
            UnsubscribeInputConfirm();
    }

    private void HandlePassPressed()
    {
        if (deadBallManager.DeadBallState != DeadBallState.Executing) return;
        if (!isBallReady) return;
        if (!deadBallManager.IsUserOffense) return;

        MarkKickExecuted();
    }

    #endregion

    #region Helpers

    private void SetPositions()
    {
        characterKicker.Teleport(deadBallManager.PositionConfig.KickoffKicker);
        characterReceiver.Teleport(deadBallManager.PositionConfig.KickoffReceiver[team.TeamSide]);
        PossessionManager.Instance.Release();

        if (deadBallManager.IsFirstKickoff) 
        {
            // warm ball on low end android devices
            // the on gain event will pass the ball to characterKicker
            PossessionManager.Instance.GiveBallToCharacter(characterReceiver);
            deadBallManager.MarkFirstKickoffComplete();
        } else 
        {
            PossessionManager.Instance.GiveBallToCharacter(characterKicker);
            PossessionManager.Instance.SetCooldown(characterKicker);
        }
    }

    #endregion
}
