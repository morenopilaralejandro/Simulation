using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChunkSceneRoot : MonoBehaviour
{
    [Header("Chunk Identity")]
    public string chunkId;
    public string zoneId;
    public Vector2Int chunkCoord;

    [Header("Spawn Points")]
    [SerializeField] private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    [Header("Gizmo Settings")]
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.5f);
    public bool showGizmo = true;

    private void Awake()
    {
        // Auto-position the entire chunk based on its coordinate
        Vector3 worldPosition = new Vector3(
            chunkCoord.x * WorldConstants.CHUNK_SIZE,
            chunkCoord.y * WorldConstants.CHUNK_SIZE,
            0f
        );
        transform.position = worldPosition;
    }

    private void Start()
    {
        SpawnPointRegistry.Instance?.RegisterSpawnPoints(zoneId, _spawnPoints.ToArray());
    }

    private void OnDestroy()
    {
        SpawnPointRegistry.Instance?.UnregisterSpawnPoints(zoneId, _spawnPoints.ToArray());
    }

    public SpawnPoint GetSpawnPoint(string spawnId)
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            if (_spawnPoints[i].spawnId == spawnId)
                return _spawnPoints[i];
        }
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Collect Spawn Points")]
    private void CollectSpawnPoints()
    {
        _spawnPoints = GetComponentsInChildren<SpawnPoint>(true).ToList();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[ChunkSceneRoot] Collected {_spawnPoints.Count} spawn points for {chunkId} at {zoneId}.");
    }
#endif

#if UNITY_EDITOR
    private Bounds GetGizmoBounds()
    {
        Vector3 center = new Vector3(
            (chunkCoord.x + 0.5f) * WorldConstants.CHUNK_SIZE,
            (chunkCoord.y + 0.5f) * WorldConstants.CHUNK_SIZE,
            0f
        );
        Vector3 size = new Vector3(
            WorldConstants.CHUNK_SIZE,
            WorldConstants.CHUNK_SIZE,
            1f
        );
        return new Bounds(center, size);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Bounds bounds = GetGizmoBounds();

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
        Gizmos.DrawCube(bounds.center, bounds.size);

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        UnityEditor.Handles.color = gizmoColor;
        UnityEditor.Handles.Label(
            bounds.min + new Vector3(0.2f, bounds.size.y - 0.5f, 0f),
            $"Chunk ({chunkCoord.x}, {chunkCoord.y})\n{chunkId}"
        );
    }

    private void OnDrawGizmosSelected()
    {
        Bounds bounds = GetGizmoBounds();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
        float size = WorldConstants.CHUNK_SIZE;
        Vector3 origin = bounds.min;
        for (int i = 1; i < size; i++)
        {
            Gizmos.DrawLine(
                origin + new Vector3(i, 0f, 0f),
                origin + new Vector3(i, size, 0f));
            Gizmos.DrawLine(
                origin + new Vector3(0f, i, 0f),
                origin + new Vector3(size, i, 0f));
        }
    }
#endif
}
