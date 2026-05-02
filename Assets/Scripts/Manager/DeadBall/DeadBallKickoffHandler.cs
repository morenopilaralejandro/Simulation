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
    private int receiverIndex;

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

        characterKicker = team.GetCharacterEntities(BattleManager.Instance.CurrentType)[team.GetFormation(BattleManager.Instance.CurrentType).Kickoff0];
        characterReceiver = team.GetCharacterEntities(BattleManager.Instance.CurrentType)[team.GetFormation(BattleManager.Instance.CurrentType).Kickoff1];
        receiverIndex = deadBallManager.CharacterSelector.GetKickoffReceiverIndex(team.GetFormation(BattleManager.Instance.CurrentType).Kickoff0, team.GetFormation(BattleManager.Instance.CurrentType).Kickoff1);

        if (team.TeamSide == BattleManager.Instance.GetUserSide())
            CharacterChangeControlManager.Instance.SetControlledCharacter(characterKicker, team.TeamSide);

        LogManager.Trace($"[DeadBallKickoffHandler] Kickoff - Kicker: {characterKicker.name}, Receiver: {characterReceiver.name}, Same: {characterKicker == characterReceiver}");

        SetPositions();

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
                target = team.GetCharacterEntities(BattleManager.Instance.CurrentType)[receiverIndex];
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
        if (deadBallManager.IsUserMenuOpen()) return; // mirror old guard

        if (isMultiplayer)
            deadBallManager.TeamReadiness.SetUserReady();
        else
            deadBallManager.TeamReadiness.SetBothReady();

        deadBallManager.NotifyReadinessChanged();

        if (deadBallManager.DeadBallState == DeadBallState.Executing)
        {
            UnsubscribeInputConfirm();

            // Only the user-offense human-kicker path needs the pass input.
            // AI-kicker / auto-battle already set isKickExecuted, and Execute()
            // already cascaded into Finish() via NotifyHandlerReady().
            bool needsOffenseInput = deadBallManager.IsUserOffense
                                  && !characterKicker.IsEnemyAI
                                  && !isKickExecuted;

            if (needsOffenseInput)
                SubscribeInputPass();
        }
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
        characterKicker.Teleport(deadBallManager.GetDefaultPositionKickoffKicker());
        characterReceiver.Teleport(deadBallManager.GetDefaultPositionKickoffReceive(team.TeamSide));
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
