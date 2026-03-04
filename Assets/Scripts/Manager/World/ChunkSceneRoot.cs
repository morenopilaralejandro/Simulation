using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChunkSceneRoot : MonoBehaviour
{
    [Header("Chunk Identity")]
    public string chunkId;
    public string zoneId;
    public Vector2Int chunkCoord;

    [Header("OverworldDefinition")]
    [SerializeField] private OverworldDefinition _overworldDefinition;

    [Header("Spawn Points")]
    [SerializeField] private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    [Header("Gizmo Settings")]
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.5f);
    public bool showGizmo = true;

    private void Awake()
    {
        SnapToChunkPosition();
    }

    private void Start()
    {
        SpawnPointRegistry.Instance?.RegisterSpawnPoints(zoneId, _spawnPoints.ToArray());
    }

    private void OnDestroy()
    {
        SpawnPointRegistry.Instance?.UnregisterSpawnPoints(zoneId, _spawnPoints.ToArray());
    }

    private void SnapToChunkPosition()
    {
        Vector3 worldPosition = new Vector3(
            chunkCoord.x * WorldConstants.CHUNK_SIZE,
            chunkCoord.y * WorldConstants.CHUNK_SIZE,
            0f
        );
        transform.position = worldPosition;
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
        // 1. Collect spawn points from children
        _spawnPoints = GetComponentsInChildren<SpawnPoint>(true).ToList();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[ChunkSceneRoot] Collected {_spawnPoints.Count} spawn points for {chunkId} at {zoneId}.");

        // 2. Update the corresponding ChunkDefinition in the OverworldDefinition
        if (_overworldDefinition == null)
        {
            Debug.LogWarning($"[ChunkSceneRoot] No OverworldDefinition assigned on {chunkId}. " +
                             $"Cannot sync spawn IDs to ChunkDefinition.");
            return;
        }

        ChunkDefinition matchingChunk = _overworldDefinition.allChunks
            .FirstOrDefault(c => c.chunkId == chunkId);

        if (matchingChunk == null)
        {
            Debug.LogWarning($"[ChunkSceneRoot] Could not find ChunkDefinition with chunkId '{chunkId}' " +
                             $"in OverworldDefinition '{_overworldDefinition.name}'.");
            return;
        }

        // 3. Sync the spawn IDs
        matchingChunk.containedSpawnIds.Clear();
        foreach (SpawnPoint sp in _spawnPoints)
        {
            if (!string.IsNullOrEmpty(sp.spawnId))
            {
                matchingChunk.containedSpawnIds.Add(sp.spawnId);
            }
            else
            {
                Debug.LogWarning($"[ChunkSceneRoot] SpawnPoint '{sp.gameObject.name}' in chunk '{chunkId}' " +
                                 $"has an empty spawnId. Skipping.");
            }
        }

        UnityEditor.EditorUtility.SetDirty(_overworldDefinition);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log($"[ChunkSceneRoot] Synced {matchingChunk.containedSpawnIds.Count} spawn IDs " +
                  $"to ChunkDefinition '{chunkId}' in OverworldDefinition '{_overworldDefinition.name}'.");
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("SnapToChunkPosition")]
    private void SnapToChunkPositionContext()
    {
        SnapToChunkPosition();
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
