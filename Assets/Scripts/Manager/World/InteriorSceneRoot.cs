using UnityEngine;
using System.Linq;

/// <summary>
/// Place this on the root GameObject of every interior scene.
/// </summary>
public class InteriorSceneRoot : MonoBehaviour
{
    [Header("Interior Identity")]
    public string zoneId;

    private SpawnPoint[] _spawnPoints;

    private void Awake()
    {
        _spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
    }

    private void Start()
    {
        SpawnPointRegistry.Instance?.RegisterSpawnPoints(zoneId, _spawnPoints);
    }

    private void OnDestroy()
    {
        if (SpawnPointRegistry.Instance != null)
        {
            SpawnPointRegistry.Instance.UnregisterSpawnPoints(zoneId, _spawnPoints);
        }
    }

    public SpawnPoint GetSpawnPoint(string spawnId)
    {
        return _spawnPoints.FirstOrDefault(sp => sp.spawnId == spawnId);
    }
}
