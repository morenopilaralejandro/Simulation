using UnityEngine;
using System.Linq;

public class ChunkSceneRoot : MonoBehaviour
{
    [Header("Chunk Identity")]
    public string chunkId;
    public string zoneId;
    public Vector2Int chunkCoord;

    private SpawnPoint[] _spawnPoints;
    private Grid _grid;

    private void Awake()
    {
        _spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
        _grid = GetComponentInChildren<Grid>();

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
        SpawnPointRegistry.Instance?.RegisterSpawnPoints(zoneId, _spawnPoints);
    }

    private void OnDestroy()
    {
        SpawnPointRegistry.Instance?.UnregisterSpawnPoints(zoneId, _spawnPoints);
    }

    public SpawnPoint GetSpawnPoint(string spawnId)
    {
        return _spawnPoints.FirstOrDefault(sp => sp.spawnId == spawnId);
    }
}
