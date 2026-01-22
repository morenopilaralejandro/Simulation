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
    private TeamSide userSide;
    private DeadBallManager deadBallManager;
    private Character characterKicker;
    private Character characterReceiver;


    private bool isBallReady;
    private bool isAutoBattleEnabled;
    private bool isMultiplayer;

    public bool IsReady => deadBallManager.AreBothTeamsReady && isBallReady;

    #endregion

    #region Interface

    public void Setup(TeamSide teamSide)
    {
        deadBallManager = DeadBallManager.Instance;
        isAutoBattleEnabled = AutoBattleManager.Instance.IsAutoBattleEnabled;
        isMultiplayer = false;
        isBallReady = false;
        userSide = BattleManager.Instance.GetUserSide();

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
        if (!InputManager.Instance.GetDown(CustomAction.Pass)) return;

        if (isMultiplayer) 
            deadBallManager.SetTeamReady(userSide);
        else 
            deadBallManager.SetBothTeamsReady();
    }

    private void OnBallGained(Character c)
    {
        if (c == characterKicker)
            isBallReady = true;

        if (isAutoBattleEnabled) 
            deadBallManager.SetBothTeamsReady();

        BallEvents.OnGained -= OnBallGained;
    }

    public void Execute()
    {
        Character target = BattleManager.Instance.TargetedCharacter[characterKicker.TeamSide];
        //CharacterChangeControlManager.Instance.SetControlledCharacter(character0, character0.TeamSide);

        if (!target || characterKicker.IsEnemyAI) 
        {
            characterKicker.KickBallTo(characterReceiver.transform.position);
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

        // warm ball on low end android devices
        if (deadBallManager.IsFirstKickoff) 
        {
            PossessionManager.Instance.GiveBallToCharacter(characterReceiver);
            characterReceiver.KickBallTo(GoalManager.Instance.Keepers[0].transform.position);
            PossessionManager.Instance.GiveBallToCharacter(characterKicker);
            deadBallManager.SetIsFirstKickoff(false);
        } else 
        {
            PossessionManager.Instance.GiveBallToCharacter(characterKicker);
        }

    }
    #endregion
}
