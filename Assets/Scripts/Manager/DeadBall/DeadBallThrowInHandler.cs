using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.DeadBall;
using Aremoreno.Enums.Input;

public class DeadBallThrowInHandler : IDeadBallHandler
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
    private float throwInCornerDistance = 3f;
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
        isBallReady = false;

        team = BattleManager.Instance.Teams[teamSide];

        ResetPositions();

        DuelLogManager.Instance.AddDeadBallThrowIn(characterKicker.Character, characterKicker.TeamSide);
    }

    public void ResetPositions()
    {
        UnsubscribeInputConfirm();  //  clear stale subs
        UnsubscribeInputPass();

        isKickExecuted = false;
        isBallReady = false;
        BallEvents.OnGained -= OnBallGained;
        deadBallManager.StopRoutine(ballReadyRoutine);

        characterKicker = deadBallManager.CharacterSelector.GetKicker(team);
        characterKicker.HasBallInHandThrowIn = true;
        characterSupportOffense = deadBallManager.CharacterSelector.GetClosestSupporters(
            deadBallManager.OffenseTeam,
            characterKicker);
        characterSupportDefense = deadBallManager.CharacterSelector.GetClosestSupporters(
            deadBallManager.DefenseTeam,
            characterKicker);

        defaultRecieverIndex = deadBallManager.CharacterSelector.GetDefaultReceiverIndex(
            characterSupportOffense, characterKicker);

        if (IsThrowInCorner())
            SetPositionsCorner();
        else
            SetPositionsDefault();

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

        //  Subscribe FIRST so cascades that end in Finish() can unsubscribe cleanly
        SubscribeInputConfirm();

        //  Set flags BEFORE any Notify* call so IsReady reflects the truth
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

        if (!target || characterKicker.IsEnemyAI)
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

        characterKicker.HasBallInHandThrowIn = false;
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

    private bool IsThrowInCorner()
    {
        return Mathf.Abs(ballPosition.z) >= throwInCornerDistance;
    }

    private void SetPositionsCorner()
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

    private void SetPositionsDefault()
    {
        BoundPlacement boundPlacement = GetBallSidePlacement(ballPosition);

        // Offensive supporters
        for (int i = 0; i < characterSupportOffense.Length && i < deadBallManager.PositionConfig.ThrowInDefaultOffense.Length; i++)
        {
            Vector3 basePos = deadBallManager.PositionConfig.ThrowInDefaultOffense[i];
            Vector3 finalPos = deadBallManager.PositionUtils.FlipPositionOnThrowIn(basePos, ballPosition, boundPlacement);

            characterSupportOffense[i].Teleport(finalPos);
        }

        // Defensive supporters
        for (int i = 0; i < characterSupportDefense.Length && i < deadBallManager.PositionConfig.ThrowInDefaultDefense.Length; i++)
        {
            Vector3 basePos = deadBallManager.PositionConfig.ThrowInDefaultDefense[i];
            Vector3 finalPos = deadBallManager.PositionUtils.FlipPositionOnThrowIn(basePos, ballPosition, boundPlacement);

            characterSupportDefense[i].Teleport(finalPos);
        }
    }

    private void SetKickerPosition()
    {
        Vector3 kickerPosition = GetKickerPosition(ballPosition);
        characterKicker.Teleport(kickerPosition);

        Quaternion rotation = GetKickerThrowInRotation(ballPosition);
        characterKicker.Model.transform.rotation = rotation;

        PossessionManager.Instance.Release();
        PossessionManager.Instance.GiveBallToCharacter(characterKicker);
        PossessionManager.Instance.SetCooldown(characterKicker);
    }

    private Vector3 GetAdjustedDefaultPosition(Vector3 basePosition)
    {
        Vector3 adjustedPosition = basePosition;
        adjustedPosition.z += ballPosition.z;
        return adjustedPosition;
    }

    public BoundPlacement GetBallSidePlacement(Vector3 ballPos)
    {
        if (ballPos.x > 0f)
            return BoundPlacement.Right;
        return BoundPlacement.Left;
    }

    private Quaternion GetKickerThrowInRotation(Vector3 ballPosition)
    {
        BoundPlacement side = GetBallSidePlacement(ballPosition);
        switch (side)
        {
            case BoundPlacement.Right:
                // Ball on right sideline → face left
                return Quaternion.Euler(0f, 270f, 0f);

            case BoundPlacement.Left:
            default:
                // Ball on left sideline → face right
                return Quaternion.Euler(0f, 90f, 0f);
        }
    }

    private Vector3 GetKickerPosition(Vector3 ballPosition)
    {
        BoundPlacement side = GetBallSidePlacement(ballPosition);
        Vector3 pos = deadBallManager.PositionConfig.CornerKicker;
        pos.z = ballPosition.z;
        switch (side)
        {
            case BoundPlacement.Right:
                pos.x *= -1;
                // Ball on right sideline → face left
                return pos;

            case BoundPlacement.Left:
            default:
                // Ball on left sideline → face right
                return pos;
        }
    }

    #endregion
}
