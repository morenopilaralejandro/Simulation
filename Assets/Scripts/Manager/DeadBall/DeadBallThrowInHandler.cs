using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;
using Simulation.Enums.Input;

public class DeadBallThrowInHandler : IDeadBallHandler
{

    #region Fields

    private Team team;
    private DeadBallManager deadBallManager;
    private Character characterKicker;
    private Character[] characterSupportOffense;
    private Character[] characterSupportDefense;
    private Vector3 ballPosition;

    private bool isBallReady;
    private bool isAutoBattleEnabled;
    private bool isMultiplayer;
    private float throwInCornerDistance = 3f;

    public bool IsReady => deadBallManager.AreBothTeamsReady && isBallReady;

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
        characterKicker = deadBallManager.GetKickerCharacter(team);
        characterSupportOffense = deadBallManager.GetClosestCharacters(
            deadBallManager.OffenseTeam,
            characterKicker);
        characterSupportDefense = deadBallManager.GetClosestCharacters(
            deadBallManager.DefenseTeam,
            characterKicker);

        if (IsThrowInCorner()) 
            SetPositionsCorner();
        else 
            SetPositionsDefault();

        SetKickerPosition();

        BallEvents.OnGained += OnBallGained;
        deadBallManager.SetState(DeadBallState.WaitingForReady);
    }

    public void HandleInput()
    {
        if (!InputManager.Instance.GetDown(CustomAction.Pass)) return;

        if (isMultiplayer) 
            deadBallManager.SetUserTeamReady();
        else 
            deadBallManager.SetBothTeamsReady();
    }

    private void OnBallGained(Character c)
    {
        if (c == characterKicker) 
        {
            isBallReady = true;
            BallEvents.OnGained -= OnBallGained;
        }

        if (isAutoBattleEnabled) 
            deadBallManager.SetBothTeamsReady();
    }

    public void Execute()
    {
        Character target = BattleManager.Instance.TargetedCharacter[characterKicker.TeamSide];

        if (!target || characterKicker.IsEnemyAI) 
        {
            target = characterSupportOffense[0];
            characterKicker.KickBallTo(target.transform.position);
            if (!characterKicker.IsEnemyAI)
                CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
        }
        else 
        {
            characterKicker.KickBallTo(target.transform.position);
            CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
        }
    }

    #endregion

    #region Helpers

    private bool IsThrowInCorner()
    {
        return Mathf.Abs(ballPosition.z) >= throwInCornerDistance;
    }

    private void SetPositionsCorner() 
    {
        CornerPlacement cornerPlacement = deadBallManager.GetBallCornerPlacement(ballPosition);

        // Offensive supporters
        for (int i = 0; i < characterSupportOffense.Length && i < deadBallManager.PositionConfig.ThrowInCornerOffense.Length; i++)
        {
            Vector3 basePos = deadBallManager.PositionConfig.ThrowInCornerOffense[i];
            Vector3 finalPos = deadBallManager.FlipPositionOnCorner(basePos, cornerPlacement);

            characterSupportOffense[i].Teleport(finalPos);
        }

        // Defensive supporters
        for (int i = 0; i < characterSupportDefense.Length && i < deadBallManager.PositionConfig.ThrowInCornerDefense.Length; i++)
        {
            Vector3 basePos = deadBallManager.PositionConfig.ThrowInCornerDefense[i];
            Vector3 finalPos = deadBallManager.FlipPositionOnCorner(basePos, cornerPlacement);

            characterSupportDefense[i].Teleport(finalPos);
        }
    }

    private void SetPositionsDefault() 
    {
        BoundPlacement boundPlacement = GetBallSidePlacement(ballPosition);

        // Offensive supporters
        for (int i = 0; i < characterSupportOffense.Length && i < deadBallManager.PositionConfig.ThrowInDefaultOffense.Length; i++)
        {
            Vector3 basePos = deadBallManager.PositionConfig.ThrowInDefaultOffense[i];
            Vector3 finalPos = FlipPositionOnThrowIn(basePos, boundPlacement);

            characterSupportOffense[i].Teleport(finalPos);
        }

        // Defensive supporters
        for (int i = 0; i < characterSupportDefense.Length && i < deadBallManager.PositionConfig.ThrowInDefaultDefense.Length; i++)
        {
            Vector3 basePos = deadBallManager.PositionConfig.ThrowInDefaultDefense[i];
            Vector3 finalPos = FlipPositionOnThrowIn(basePos, boundPlacement);

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
        switch (side)
        {
            case BoundPlacement.Right:
                // Ball on right sideline → face left
                return new Vector3 (6.5f, 0.35f, ballPosition.z);

            case BoundPlacement.Left:
            default:
                // Ball on left sideline → face right
                return new Vector3 (-6.5f, 0.35f, ballPosition.z);
        }
    }

    public Vector3 FlipPositionOnThrowIn(Vector3 basePos, BoundPlacement boundPlacement)
    {
        Vector3 pos = GetAdjustedDefaultPosition(basePos);

        switch (boundPlacement)
        {
            case BoundPlacement.Left:
                pos.x *= 1f;
                break;

            case BoundPlacement.Right:
                pos.x *= -1f;
                break;
        }

        return pos;
    }

    #endregion

}
