using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;
using Simulation.Enums.Input;

public class DeadBallKickoffHandler : IDeadBallHandler
{
    #region Fields

    private Team team;
    private DeadBallManager deadBallManager;
    private Character characterKicker;
    private Character characterReceiver;

    private bool isBallReady;
    private bool isAutoBattleEnabled;
    private bool isMultiplayer;

    public bool IsReady => deadBallManager.TeamReadiness.AreBothReady && isBallReady;

    #endregion

    #region Interface

    public void Setup(TeamSide teamSide)
    {
        deadBallManager = DeadBallManager.Instance;
        isAutoBattleEnabled = AutoBattleManager.Instance.IsAutoBattleEnabled;
        isMultiplayer = false;
        isBallReady = false;

        team = BattleManager.Instance.Teams[teamSide];
        characterKicker = team.CharacterList[team.Formation.Kickoff0];
        characterReceiver = team.CharacterList[team.Formation.Kickoff1];

        SetPositions();

        AudioManager.Instance.PlaySfx("sfx-whistle_single");
        DuelLogManager.Instance.AddDeadBallKickoff(characterKicker);

        BallEvents.OnGained += OnBallGained;
        deadBallManager.SetState(DeadBallState.WaitingForReady);
    }

    public void HandleInput()
    {
        if (!InputManager.Instance.GetDown(CustomAction.Battle_Pass)) return;

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
        } else 
        {
            c.KickBallTo(characterKicker.transform.position);
        }

        if (isAutoBattleEnabled) 
            deadBallManager.TeamReadiness.SetBothReady();
    }

    public void Execute()
    {
        Character target = BattleManager.Instance.TargetedCharacter[characterKicker.TeamSide];

        if (!target || characterKicker.IsEnemyAI) 
        {
            characterKicker.KickBallTo(characterReceiver.transform.position);
            CharacterChangeControlManager.Instance.TryChangeOnDeadBallGeneric(characterReceiver);
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
        }
    }


    #endregion
}
