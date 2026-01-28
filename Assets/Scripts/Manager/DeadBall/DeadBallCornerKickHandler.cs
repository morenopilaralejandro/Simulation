using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;
using Simulation.Enums.Input;

public class DeadBallCornerKickHandler : IDeadBallHandler
{

    #region Fields

    private Team team;
    private DeadBallManager deadBallManager;
    private Character characterKicker;
    private Character[] characterSupportOffense;
    private Character[] characterSupportDefense;
    private Vector3 ballPosition;
    private int defaultRecieverIndex;

    private bool isBallReady;
    private bool isAutoBattleEnabled;
    private bool isMultiplayer;

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

        defaultRecieverIndex = deadBallManager.GetDefaultRecieverIndexInArray(characterSupportOffense, characterKicker);

        SetPositions();
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
            target = characterSupportOffense[defaultRecieverIndex];
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

    private void SetPositions() 
    {
        CornerPlacement cornerPlacement = deadBallManager.GetBallCornerPlacement(ballPosition);

        deadBallManager.SetCornerPositions(
            characterSupportOffense,
            deadBallManager.PositionConfig.ThrowInCornerOffense,
            deadBallManager.OffenseTeam.TeamSide,
            cornerPlacement
        );

        deadBallManager.SetCornerPositions(
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
    }

    private Quaternion GetKickerCornerRotation(Vector3 ballPosition)
    {
        CornerPlacement corner = deadBallManager.GetBallCornerPlacement(ballPosition);

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
        CornerPlacement cornerPlacement = deadBallManager.GetBallCornerPlacement(ballPosition);
        Vector3 basePos = deadBallManager.PositionConfig.CornerKicker;
        Vector3 finalPos = deadBallManager.FlipPositionOnCorner(basePos, cornerPlacement);
        return finalPos;
    }

    #endregion

}
