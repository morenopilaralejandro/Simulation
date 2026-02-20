using UnityEngine;

/// <summary>
/// Place on a trigger collider in the scene. When the player enters,
/// it transitions to the connected zone/spawn.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZoneTransitionTrigger : MonoBehaviour
{
    [Header("Destination")]
    public ZoneDefinition targetZone;
    public string targetSpawnId;

    [Header("Transition Settings")]
    public float transitionDelay = 0.2f;
    public bool requireInteraction = false; // If true, player must press a button

    //private bool _playerInTrigger = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (requireInteraction)
        {
            //_playerInTrigger = true;
            // Show interaction prompt via UI
            //UIManager.Instance?.ShowInteractionPrompt("Enter");
        }
        else
        {
            InitiateTransition();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        //_playerInTrigger = false;
        //UIManager.Instance?.HideInteractionPrompt();
    }

    private void InitiateTransition()
    {
        //_playerInTrigger = false;
        WorldManager.Instance.TransitionToZone(targetZone.zoneId, targetSpawnId);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        var col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawCube(
                transform.position + (Vector3)box.offset,
                box.size
            );
        }

        #if UNITY_EDITOR
        string label = targetZone != null
            ? $"→ {targetZone.zoneName} ({targetSpawnId})"
            : "No target set!";
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.7f, label);
        #endif
    }
}
