using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

[RequireComponent(typeof(BoxCollider))]
public class Bound : MonoBehaviour
{
    [SerializeField] private BoundPlacement boundPlacement; // Assign from inspector
    private BoxCollider boxCollider;

    public BoundPlacement BoundPlacement => boundPlacement;
    public BoxCollider BoxCollider => boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        BoundManager.Instance?.RegisterBound(this);
    }

    private void OnDestroy()
    {
        BoundManager.Instance?.UnregisterBound(this);
    }
}
