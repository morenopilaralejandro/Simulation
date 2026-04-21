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

    private bool isBallReady;
    private bool isAutoBattleEnabled;
    private bool isMultiplayer;
    private Coroutine ballReadyRoutine;

    public bool IsReady => deadBallManager.TeamReadiness.AreBothReady && isBallReady;
    


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

        DuelLogManager.Instance.AddDeadBallCornerKick(characterKicker);
    }


    public void ResetPositions() 
    {
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

        defaultRecieverIndex = deadBallManager.CharacterSelector.GetDefaultReceiverIndex(characterSupportOffense, characterKicker);
        SetPositions();
        SetKickerPosition();

        BallEvents.OnGained += OnBallGained;
    }

    public void HandleInput()
    {
        if (!InputManager.Instance.GetDown(CustomAction.Battle_Pass)) return;
        if (!isBallReady) return;

        if (isMultiplayer) 
            deadBallManager.TeamReadiness.SetUserReady();
        else 
            deadBallManager.TeamReadiness.SetBothReady();
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

        if (isAutoBattleEnabled) 
            deadBallManager.TeamReadiness.SetBothReady();
    }

    public void Execute()
    {
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
            case CornerPlacement.TopLeft:
                return Quaternion.Euler(0f, 135f, 0f);

            case CornerPlacement.TopRight:
                return Quaternion.Euler(0f, 225f, 0f);

            case CornerPlacement.BottomLeft:
                return Quaternion.Euler(0f, 45f, 0f);

            case CornerPlacement.BottomRight:
                return Quaternion.Euler(0f, 315f, 0f);

            default:
                return Quaternion.identity;
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
