using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

[RequireComponent(typeof(BoxCollider))]
public class BoundOut : MonoBehaviour
{
    [SerializeField] private BoundType boundType;

    #region Unity Lifecycle

    private void OnCollisionEnter(Collision other)
    {
        HandleCollision(other);
    }

    private void OnCollisionStay(Collision other)
    {
        HandleCollision(other);
    }

    #endregion

    #region Duel Logic

    private void HandleCollision(Collision other) 
    {
        if (BattleManager.Instance.IsTimeFrozen) return;

        GameObject hitObj = other.collider.gameObject;
        if (!hitObj.CompareTag("Ball")) return;

        //var ball = BattleManager.Instance.Ball;

        //freeze
        //whistle sfx

        switch (boundType)
        {
            case BoundType.Endline:
                Debug.Log("Ball crossed the end line.");
                break;

            case BoundType.Sideline:
                Debug.Log("Ball went out on the sideline.");
                break;

        }

    }

    #endregion
}
