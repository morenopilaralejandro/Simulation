using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Collider2D))]
public class InteractableComponentCollider : MonoBehaviour
{
    #region Fields

    [SerializeField] private Interactable interactable;
    public Interactable Interactable => interactable;

    #endregion

    #region Unity Lifecycle

    /*
    private void OnTriggerEnter(Collider other)
    {
        TryStartDuel(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryStartDuel(other);
    }
    */

    #endregion

    #region Logic

    #endregion
}
