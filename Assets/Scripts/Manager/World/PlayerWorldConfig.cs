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

    [Header("Interaction")]
    public float interactionRange = 1.5f;
    public LayerMask interactableLayer;
}
