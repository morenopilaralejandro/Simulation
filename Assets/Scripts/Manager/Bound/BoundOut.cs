using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

[RequireComponent(typeof(BoxCollider))]
public class BoundOut : MonoBehaviour
{
    [SerializeField] private BoundType boundType;
    private BattleManager battleManager;
    private DeadBallManager deadBallManager;
    private AudioManager audioManager;

    #region Unity Lifecycle

    private void Start() 
    {
        battleManager = BattleManager.Instance;
        deadBallManager = DeadBallManager.Instance;
        audioManager = AudioManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleTrigger(other);
    }

    #endregion

    #region Duel Logic

    private void HandleTrigger(Collider other) 
    {
        if (battleManager.IsTimeFrozen) return;

        GameObject hitObj = other.GetComponent<Collider>().gameObject;
        if (!hitObj.CompareTag("Ball")) return;

        audioManager.PlaySfx("sfx-whistle_single");

        battleManager.Freeze();
        var ball = battleManager.Ball;
        deadBallManager.SetBallPosition(ball.transform.position);
        TeamSide teamSide = deadBallManager.GetRestartTeamSide();
        //freeze
        //whistle sfx

        switch (boundType)
        {
            case BoundType.Endline:
                LogManager.Trace("[BoundOut] [HandleTrigger] Ball crossed the endline.");
                //determine if it is corner or goal kick
                if(deadBallManager.IsCornerKick(teamSide))
                    battleManager.StartCornerKick(teamSide);
                else
                    battleManager.StartGoalKick(teamSide);
                break;

            case BoundType.Sideline:
                LogManager.Trace("[BoundOut] [HandleTrigger] Ball crossed the sideline.");
                battleManager.StartThrowIn(teamSide);
                break;

        }

    }

    #endregion
}
