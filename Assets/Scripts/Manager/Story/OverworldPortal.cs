using UnityEngine;

/// <summary>
/// Place this on a trigger collider in any chunk scene.
/// Transports player between overworlds (Earth ↔ Heaven ↔ Hell).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class OverworldPortal : MonoBehaviour
{
    /*
    [Header("Destination")]
    public ZoneDefinition targetOverworld;
    public string targetSpawnId;

    [Header("Requirements (optional)")]
    public bool requiresItem = false;
    public string requiredItemId;
    public bool requiresInteraction = true;

    [Header("Visual")]
    public GameObject portalEffect;
    public Color gizmoColor = Color.magenta;

    private bool _playerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!requiresInteraction)
        {
            TryTransition();
            return;
        }

        _playerInRange = true;
        UIManager.Instance?.ShowInteractionPrompt("Enter Portal");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;
        UIManager.Instance?.HideInteractionPrompt();
    }

    private void Update()
    {
        if (_playerInRange && requiresInteraction && Input.GetKeyDown(KeyCode.E))
        {
            TryTransition();
        }
    }

    private void TryTransition()
    {
        // Check item requirement
        if (requiresItem)
        {
            // Replace with your inventory system
            bool hasItem = InventoryManager.Instance != null 
                && InventoryManager.Instance.HasItem(requiredItemId);

            if (!hasItem)
            {
                UIManager.Instance?.ShowMessage("You need a special item to enter.");
                return;
            }
        }

        _playerInRange = false;
        WorldManager.Instance.TransitionToZone(
            targetOverworld.zoneId, 
            targetSpawnId
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        var col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawCube(
                transform.position + (Vector3)box.offset,
                box.size
            );
        }

        #if UNITY_EDITOR
        string label = targetOverworld != null
            ? $"PORTAL → {targetOverworld.zoneName}\n({targetSpawnId})"
            : "No destination!";
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 1f, 
            label
        );
        #endif
    }
    */
}
