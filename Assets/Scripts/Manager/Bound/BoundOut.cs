using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

[RequireComponent(typeof(BoxCollider))]
public class BoundOut : MonoBehaviour
{
    [SerializeField] private BoundType boundType;

    #region Unity Lifecycle

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
        if (BattleManager.Instance.IsTimeFrozen) return;

        GameObject hitObj = other.GetComponent<Collider>().gameObject;
        if (!hitObj.CompareTag("Ball")) return;

        BattleManager.Instance.Freeze();
        var ball = BattleManager.Instance.Ball;
        DeadBallManager.Instance.SetBallPosition(ball.transform.position);
        TeamSide teamSide = DeadBallManager.Instance.GetDeadBallSide();
        //freeze
        //whistle sfx

        switch (boundType)
        {
            case BoundType.Endline:
                LogManager.Trace("[BoundOut] [HandleTrigger] Ball crossed the endline.");
                //determine if it is corner or goal kick
                if(DeadBallManager.Instance.IsCornerKick(teamSide))
                    BattleManager.Instance.StartCornerKick(teamSide);
                else
                    Debug.LogError($"Goal kick {teamSide}");
                    //BattleManager.Instance.StartCornerKick(teamSide);
                break;

            case BoundType.Sideline:
                LogManager.Trace("[BoundOut] [HandleTrigger] Ball crossed the sideline.");
                BattleManager.Instance.StartThrowIn(teamSide);
                break;

        }

    }

    #endregion
}
