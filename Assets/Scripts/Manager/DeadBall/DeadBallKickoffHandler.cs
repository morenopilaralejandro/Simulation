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
        characterKicker = team.GetCharacterEntities(BattleManager.Instance.CurrentType)[team.GetFormation(BattleManager.Instance.CurrentType).Kickoff0];
        characterReceiver = team.GetCharacterEntities(BattleManager.Instance.CurrentType)[team.GetFormation(BattleManager.Instance.CurrentType).Kickoff1];
        receiverIndex = deadBallManager.CharacterSelector.GetKickoffReceiverIndex(team.GetFormation(BattleManager.Instance.CurrentType).Kickoff0, team.GetFormation(BattleManager.Instance.CurrentType).Kickoff1);

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

    private void OnBallGained(CharacterEntityBattle c)
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
        CharacterEntityBattle target = BattleManager.Instance.TargetedCharacter[characterKicker.TeamSide];

        if (!target || characterKicker.IsEnemyAI) 
        {
            if (characterKicker.IsEnemyAI)
                target = team.GetCharacterEntities(BattleManager.Instance.CurrentType)[receiverIndex];
            else
                target = characterReceiver;

            characterKicker.KickBallTo(target.transform.position);
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
