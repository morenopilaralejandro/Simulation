using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Place this on the root GameObject of every interior scene.
/// </summary>
public class InteriorSceneRoot : MonoBehaviour
{
    [Header("Interior Identity")]
    public string zoneId;

    [Header("Spawn Points")]
    [SerializeField] private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

    [Header("Gizmo Settings")]
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.5f);
    public bool showGizmo = true;

    private void Awake()
    {

    }

    private void Start()
    {
        SpawnPointRegistry.Instance?.RegisterSpawnPoints(zoneId, _spawnPoints.ToArray());
    }

    private void OnDestroy()
    {
        if (SpawnPointRegistry.Instance != null)
        {
            SpawnPointRegistry.Instance.UnregisterSpawnPoints(zoneId, _spawnPoints.ToArray());
        }
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
        Debug.Log($"[InteriorSceneRoot] Collected {_spawnPoints.Count} spawn points for {zoneId}.");
    }
#endif


#if UNITY_EDITOR
    private Bounds GetGizmoBounds()
    {
        Vector3 center = new Vector3(
            (0 + 0.5f) * WorldConstants.INTERIOR_SIZE,
            (0 + 0.5f) * WorldConstants.INTERIOR_SIZE,
            0f
        );
        Vector3 size = new Vector3(
            WorldConstants.INTERIOR_SIZE,
            WorldConstants.INTERIOR_SIZE,
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
            $"{zoneId}"
        );
    }

    private void OnDrawGizmosSelected()
    {
        Bounds bounds = GetGizmoBounds();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
        float size = WorldConstants.INTERIOR_SIZE;
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
