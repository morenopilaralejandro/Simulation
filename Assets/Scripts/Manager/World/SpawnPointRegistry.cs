using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Simulation.Enums.World;

/// <summary>
/// Central registry that tracks all currently loaded spawn points.
/// Spawn points register/unregister themselves as scenes load/unload.
/// </summary>
public class SpawnPointRegistry : MonoBehaviour
{
    public static SpawnPointRegistry Instance { get; private set; }

    // zoneId -> list of active spawn points in that zone
    private Dictionary<string, List<SpawnPoint>> _spawnPointsByZone
        = new Dictionary<string, List<SpawnPoint>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterSpawnPoints(string zoneId, SpawnPoint[] points)
    {
        if (!_spawnPointsByZone.ContainsKey(zoneId))
        {
            _spawnPointsByZone[zoneId] = new List<SpawnPoint>();
        }

        foreach (var sp in points)
        {
            if (!_spawnPointsByZone[zoneId].Contains(sp))
            {
                _spawnPointsByZone[zoneId].Add(sp);
                LogManager.Trace($"[SpawnRegistry] Registered spawn '{sp.spawnId}' in zone '{zoneId}'");
            }
        }
    }

    public void UnregisterSpawnPoints(string zoneId, SpawnPoint[] points)
    {
        if (!_spawnPointsByZone.ContainsKey(zoneId)) return;

        foreach (var sp in points)
        {
            _spawnPointsByZone[zoneId].Remove(sp);
        }

        if (_spawnPointsByZone[zoneId].Count == 0)
        {
            _spawnPointsByZone.Remove(zoneId);
        }
    }

    public SpawnPoint FindSpawnPoint(string zoneId, string spawnId)
    {
        if (_spawnPointsByZone.TryGetValue(zoneId, out var points))
        {
            return points.FirstOrDefault(sp => sp.spawnId == spawnId);
        }
        return null;
    }

    public SpawnPoint FindSpawnPointByType(string zoneId, SpawnPointType type)
    {
        if (_spawnPointsByZone.TryGetValue(zoneId, out var points))
        {
            return points.FirstOrDefault(sp => sp.spawnType == type);
        }
        return null;
    }

    public List<SpawnPoint> GetAllSpawnPoints(string zoneId)
    {
        if (_spawnPointsByZone.TryGetValue(zoneId, out var points))
        {
            return new List<SpawnPoint>(points);
        }
        return new List<SpawnPoint>();
    }

    public void Clear()
    {
        _spawnPointsByZone.Clear();
    }
}
