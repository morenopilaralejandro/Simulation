using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

[RequireComponent(typeof(BoxCollider))]
public class Bound : MonoBehaviour
{
    [SerializeField] private BoundPlacement boundPlacement; // Assign from inspector
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();

        if (BoundManager.Top == null && boundPlacement == BoundPlacement.Top)
        {
            BoundManager.Top = boxCollider;
        }
        else if (BoundManager.Bottom == null && boundPlacement == BoundPlacement.Bottom)
        {
            BoundManager.Bottom = boxCollider;
        }
        else if (BoundManager.Left == null && boundPlacement == BoundPlacement.Left)
        {
            BoundManager.Left = boxCollider;
        }
        else if (BoundManager.Right == null && boundPlacement == BoundPlacement.Right)
        {
            BoundManager.Right = boxCollider;
        }
    }

    private void OnDestroy()
    {
        // Ensure BoundManager releases references when these objects are destroyed
        switch (boundPlacement)
        {
            case BoundPlacement.Top:
                if (BoundManager.Top == boxCollider) BoundManager.Top = null;
                break;
            case BoundPlacement.Bottom:
                if (BoundManager.Bottom == boxCollider) BoundManager.Bottom = null;
                break;
            case BoundPlacement.Left:
                if (BoundManager.Left == boxCollider) BoundManager.Left = null;
                break;
            case BoundPlacement.Right:
                if (BoundManager.Right == boxCollider) BoundManager.Right = null;
                break;
        }
    }
}
