using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWorldConfig", menuName = "ScriptableObject/Config/PlayerWorldConfig")]
public class PlayerWorldConfig : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public bool allowDiagonalMovement = true;
    public bool gridBasedMovement = true;
    public float gridSize = 1f;

    [Header("Collision")]
    public LayerMask collisionMask;   // obstacles
    public float collisionCastRadius = 0.4f;   // slightly smaller than half a tile

    [Header("Interaction")]
    public LayerMask interactableLayer;
    public float interactionRange = 0.75f;
    public float castRadius = 0.3f;
    public int detectionInterval = 5;

    [Header("Encounter")]
    public int minStepsBetweenEncounters = 10;
    public int maxStepsBetweenEncounters = 30;
}
