using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

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
        //freeze
        //whistle sfx

        switch (boundType)
        {
            case BoundType.Endline:
                Debug.LogWarning("Ball crossed the end line.");
                //determine if it is corner or goal kick
                BattleManager.Instance.StartCornerKick(DeadBallManager.Instance.GetDeadBallSide());
                break;

            case BoundType.Sideline:
                Debug.LogWarning("Ball went out on the sideline.");
                BattleManager.Instance.StartThrowIn(DeadBallManager.Instance.GetDeadBallSide());
                break;

        }

    }

    #endregion
}
