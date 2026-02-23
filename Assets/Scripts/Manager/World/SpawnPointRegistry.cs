using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.World;

/// <summary>
/// Central registry that tracks all currently loaded spawn points.
/// Spawn points register/unregister themselves as scenes load/unload.
/// </summary>
public class SpawnPointRegistry : MonoBehaviour
{
    public static SpawnPointRegistry Instance { get; private set; }

    // zoneId -> list of active spawn points in that zone
    private readonly Dictionary<string, List<SpawnPoint>> _spawnPointsByZone
        = new Dictionary<string, List<SpawnPoint>>();

    // Reusable empty list returned by GetAllSpawnPoints to avoid allocation
    private static readonly List<SpawnPoint> EmptyList = new List<SpawnPoint>(0);

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
        List<SpawnPoint> list;
        if (!_spawnPointsByZone.TryGetValue(zoneId, out list))
        {
            list = new List<SpawnPoint>(points.Length);
            _spawnPointsByZone[zoneId] = list;
        }

        for (int i = 0, len = points.Length; i < len; i++)
        {
            SpawnPoint sp = points[i];

            // Linear Contains check — fine for small lists
            bool alreadyRegistered = false;
            for (int j = 0, count = list.Count; j < count; j++)
            {
                if (list[j] == sp)
                {
                    alreadyRegistered = true;
                    break;
                }
            }

            if (!alreadyRegistered)
            {
                list.Add(sp);
                LogManager.Trace(string.Concat(
                    "[SpawnRegistry] Registered spawn '", sp.spawnId,
                    "' in zone '", zoneId, "'"));
            }
        }
    }

    public void UnregisterSpawnPoints(string zoneId, SpawnPoint[] points)
    {
        List<SpawnPoint> list;
        if (!_spawnPointsByZone.TryGetValue(zoneId, out list)) return;

        for (int i = 0, len = points.Length; i < len; i++)
        {
            list.Remove(points[i]);
        }

        if (list.Count == 0)
        {
            _spawnPointsByZone.Remove(zoneId);
        }
    }

    public SpawnPoint FindSpawnPoint(string zoneId, string spawnId)
    {
        List<SpawnPoint> list;
        if (_spawnPointsByZone.TryGetValue(zoneId, out list))
        {
            for (int i = 0, count = list.Count; i < count; i++)
            {
                if (list[i].spawnId == spawnId)
                    return list[i];
            }
        }
        return null;
    }

    public SpawnPoint FindSpawnPointByType(string zoneId, SpawnPointType type)
    {
        List<SpawnPoint> list;
        if (_spawnPointsByZone.TryGetValue(zoneId, out list))
        {
            for (int i = 0, count = list.Count; i < count; i++)
            {
                if (list[i].spawnType == type)
                    return list[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the internal list directly. Do NOT modify the returned list.
    /// If you need a safe copy, use GetAllSpawnPointsCopy instead.
    /// </summary>
    public List<SpawnPoint> GetAllSpawnPoints(string zoneId)
    {
        List<SpawnPoint> list;
        if (_spawnPointsByZone.TryGetValue(zoneId, out list))
        {
            return list;
        }
        return EmptyList;
    }

    /// <summary>
    /// Returns a new copy of the spawn point list for safe mutation.
    /// </summary>
    public List<SpawnPoint> GetAllSpawnPointsCopy(string zoneId)
    {
        List<SpawnPoint> list;
        if (_spawnPointsByZone.TryGetValue(zoneId, out list))
        {
            return new List<SpawnPoint>(list);
        }
        return new List<SpawnPoint>(0);
    }

    public void Clear()
    {
        _spawnPointsByZone.Clear();
    }
}
