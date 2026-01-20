using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

[RequireComponent(typeof(BoxCollider))]
public class TriggerCancelBallTravel : MonoBehaviour
{
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
        if (!other.CompareTag("Ball")) return;

        var ball = BattleManager.Instance.Ball;
        if (ball.IsTraveling)
            ball.CancelTravel();
    }

    #endregion
}
