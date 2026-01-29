using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;
using Simulation.Enums.Input;

public class DeadBallFreeKickIndirectHandler : IDeadBallHandler
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

    public bool IsReady => deadBallManager.TeamReadiness.AreBothReady && isBallReady;

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
        characterKicker = deadBallManager.CharacterSelector.GetKickerIndirectFreeKick(team);
        /*
        characterSupportOffense = deadBallManager.CharacterSelector.GetClosestSupporters(
            deadBallManager.OffenseTeam,
            characterKicker);
        characterSupportDefense = deadBallManager.CharacterSelector.GetClosestSupporters(
            deadBallManager.DefenseTeam,
            characterKicker);
        */

        SetKickerPosition();

        DuelLogManager.Instance.AddDeadBallFreeKickIndirect(characterKicker);

        BallEvents.OnGained += OnBallGained;
        deadBallManager.SetState(DeadBallState.WaitingForReady);
    }

    public void HandleInput()
    {
        if (!InputManager.Instance.GetDown(CustomAction.Pass)) return;

        if (isMultiplayer) 
            deadBallManager.TeamReadiness.SetUserReady();
        else 
            deadBallManager.TeamReadiness.SetBothReady();
    }

    private void OnBallGained(Character c)
    {
        if (c == characterKicker) 
        {
            isBallReady = true;
            BallEvents.OnGained -= OnBallGained;
        }

        if (isAutoBattleEnabled) 
            deadBallManager.TeamReadiness.SetBothReady();
    }

    public void Execute()
    {
        Character target = BattleManager.Instance.TargetedCharacter[characterKicker.TeamSide];

        if (!target || characterKicker.IsEnemyAI) 
        {
            target = characterKicker.GetBestPassTeammate();
            if (!target) 
                target = deadBallManager.CharacterSelector.GetClosestTeammate(characterKicker);
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
        /*
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
        */
    }

    private void SetKickerPosition()
    {
        Vector3 kickerPosition = ballPosition;       

        kickerPosition.x = Mathf.Clamp(kickerPosition.x, -4f, 4f);
        kickerPosition.z = Mathf.Clamp(kickerPosition.z, -6.5f, 6.5f);

        characterKicker.Teleport(kickerPosition);

        //Quaternion rotation = GetKickerThrowInRotation(ballPosition);
        //characterKicker.Model.transform.rotation = rotation;

        PossessionManager.Instance.Release();
        PossessionManager.Instance.GiveBallToCharacter(characterKicker);
    }

    #endregion

}
