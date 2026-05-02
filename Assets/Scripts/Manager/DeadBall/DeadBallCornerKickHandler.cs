using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.DeadBall;
using Aremoreno.Enums.Input;

public class DeadBallCornerKickHandler : IDeadBallHandler
{
    #region Fields

    private Team team;
    private DeadBallManager deadBallManager;
    private CharacterEntityBattle characterKicker;
    private CharacterEntityBattle[] characterSupportOffense;
    private CharacterEntityBattle[] characterSupportDefense;
    private Vector3 ballPosition;
    private int defaultRecieverIndex;

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

        ballPosition = deadBallManager.CachedBallPosition;

        isAutoBattleEnabled = AutoBattleManager.Instance.IsAutoBattleEnabled;
        isMultiplayer = false;

        team = BattleManager.Instance.Teams[teamSide];

        ResetPositions();

        DuelLogManager.Instance.AddDeadBallCornerKick(characterKicker.Character, characterKicker.TeamSide);
    }

    public void ResetPositions()
    {
        UnsubscribeInputConfirm();
        UnsubscribeInputPass();

        isKickExecuted = false;
        isBallReady = false;
        BallEvents.OnGained -= OnBallGained;
        deadBallManager.StopRoutine(ballReadyRoutine);

        characterKicker = deadBallManager.CharacterSelector.GetKicker(team);
        characterSupportOffense = deadBallManager.CharacterSelector.GetClosestSupporters(
            deadBallManager.OffenseTeam,
            characterKicker);
        characterSupportDefense = deadBallManager.CharacterSelector.GetClosestSupporters(
            deadBallManager.DefenseTeam,
            characterKicker);

        defaultRecieverIndex = deadBallManager.CharacterSelector.GetDefaultReceiverIndex(
            characterSupportOffense, characterKicker);

        SetPositions();
        SetKickerPosition();

        BallEvents.OnGained += OnBallGained;
    }

    private void OnBallGained(CharacterEntityBattle c)
    {
        if (c == characterKicker)
        {
            BallEvents.OnGained -= OnBallGained;
            ballReadyRoutine = deadBallManager.StartRoutine(DelayedBallReady());
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
            target = characterSupportOffense[defaultRecieverIndex];
            characterKicker.KickBallTo(target.transform.position);
            if (!characterKicker.IsEnemyAI)
                CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
            else
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
        if (InputManager.Instance == null) return;
        InputManager.Instance.UnsubscribeDown(CustomAction.BattleUI_DeadBallConfirm, HandleConfirmPressed);
    }

    private void SubscribeInputPass()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Battle_Pass, HandlePassPressed);
    }

    private void UnsubscribeInputPass()
    {
        if (InputManager.Instance == null) return;
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
        CornerPlacement cornerPlacement = deadBallManager.PositionUtils.GetBallCornerPlacement(ballPosition);

        deadBallManager.PositionUtils.SetCornerPositions(
            characterSupportOffense,
            deadBallManager.PositionConfig.ThrowInCornerOffense,
            deadBallManager.OffenseTeam.TeamSide,
            cornerPlacement
        );

        deadBallManager.PositionUtils.SetCornerPositions(
            characterSupportDefense,
            deadBallManager.PositionConfig.ThrowInCornerDefense,
            deadBallManager.DefenseTeam.TeamSide,
            cornerPlacement
        );
    }

    private void SetKickerPosition()
    {
        Vector3 kickerPosition = GetKickerPosition(ballPosition);
        characterKicker.Teleport(kickerPosition);

        Quaternion rotation = GetKickerCornerRotation(ballPosition);
        characterKicker.Model.transform.rotation = rotation;

        PossessionManager.Instance.Release();
        PossessionManager.Instance.GiveBallToCharacter(characterKicker);
        PossessionManager.Instance.SetCooldown(characterKicker);
    }

    private Quaternion GetKickerCornerRotation(Vector3 ballPosition)
    {
        CornerPlacement corner = deadBallManager.PositionUtils.GetBallCornerPlacement(ballPosition);

        switch (corner)
        {
            case CornerPlacement.TopLeft:     return Quaternion.Euler(0f, 135f, 0f);
            case CornerPlacement.TopRight:    return Quaternion.Euler(0f, 225f, 0f);
            case CornerPlacement.BottomLeft:  return Quaternion.Euler(0f, 45f, 0f);
            case CornerPlacement.BottomRight: return Quaternion.Euler(0f, 315f, 0f);
            default: return Quaternion.identity;
        }
    }

    private Vector3 GetKickerPosition(Vector3 ballPosition)
    {
        CornerPlacement cornerPlacement = deadBallManager.PositionUtils.GetBallCornerPlacement(ballPosition);
        Vector3 basePos = deadBallManager.PositionConfig.CornerKicker;
        Vector3 finalPos = deadBallManager.PositionUtils.FlipPositionOnCorner(basePos, cornerPlacement);
        return finalPos;
    }

    #endregion
}
