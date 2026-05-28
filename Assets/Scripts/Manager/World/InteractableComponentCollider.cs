using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Collider2D))]
public class InteractableComponentCollider : MonoBehaviour
{
    #region Fields

    private IInteractable interactable;
    public IInteractable Interactable => interactable;

    private void Awake()
    {
        interactable = GetComponent<IInteractable>();
        if (interactable == null) LogManager.Error("[InteractableComponentCollider] No IInteractable found!");
    }


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
