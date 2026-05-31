using UnityEngine;
using Aremoreno.Enums.Animation;
using Aremoreno.Enums.World;

/// <summary>
/// Place this on GameObjects in your chunk/interior scenes.
/// Position it using the Unity editor — no hardcoding needed.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Configuration")]
    public string spawnId;
    public SpawnPointType spawnType = SpawnPointType.Default;
    public CharacterDirection facingDirection = CharacterDirection.Down;
    //private Vector2 facingDirection = Vector2.down;

    [Header("Connection (for doors/transitions)")]
    [Tooltip("The zone this spawn point leads TO when used as an exit")]
    public ZoneDefinition connectedZone;
    [Tooltip("The spawn ID in the connected zone to arrive at")]
    public string connectedSpawnId;

    [Header("Visuals (Editor Only)")]
    public Color gizmoColor = Color.green;

    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }

    public Vector2 FacingToVector(CharacterDirection dir) => dir switch
    {
        CharacterDirection.Up    => Vector2.up,
        CharacterDirection.Down  => Vector2.down,
        CharacterDirection.Left  => Vector2.left,
        CharacterDirection.Right => Vector2.right,
        _                     => Vector2.down
    };

    public SpawnPointData ToData()
    {
        return new SpawnPointData
        {
            spawnId = this.spawnId,
            type = this.spawnType,
            position = transform.position,
            facingDirection = FacingToVector(this.facingDirection),
            connectedZoneId = connectedZone != null ? connectedZone.zoneId : "",
            connectedSpawnId = this.connectedSpawnId
        };
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        // Draw facing direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (Vector3)FacingToVector(facingDirection) * 0.5f);

        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.5f,
            $"{spawnId} ({spawnType})"
        );

    }
#endif
}
