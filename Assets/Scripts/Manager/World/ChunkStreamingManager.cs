using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Simulation.Enums.World;


/// <summary>
/// Manages streaming of overworld chunks based on player position.
/// Only loads chunks within the configured radius.
/// </summary>
public class ChunkStreamingManager : MonoBehaviour
{
    public static ChunkStreamingManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("How often to check player position for chunk updates (seconds)")]
    private float updateInterval = 1f;

    private ZoneDefinition _currentOverworldZone;
    private Vector2Int _lastPlayerChunkCoord = new Vector2Int(int.MinValue, int.MinValue);
    private float _updateTimer;
    private bool _isActive = false;
    private bool _isUpdating = false;

    // Track which chunks are currently loaded
    private HashSet<string> _loadedChunkIds = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Start streaming chunks for a given overworld zone.
    /// </summary>
    public void StartStreaming(ZoneDefinition zone)
    {
        if (zone.zoneType != ZoneType.Overworld)
        {
            LogManager.Error("[ChunkStreamingManager] Cannot stream a non-overworld zone!");
            return;
        }

        _currentOverworldZone = zone;
        _lastPlayerChunkCoord = new Vector2Int(int.MinValue, int.MinValue);
        _isActive = true;
        _updateTimer = 0f;

        LogManager.Trace($"[ChunkStreamingManager] Started streaming for zone: {zone.zoneName}");

        // Force immediate update
        _ = UpdateChunksAroundPlayer();
    }

    /// <summary>
    /// Stop streaming and unload all chunks.
    /// </summary>
    public async Task StopStreaming()
    {
        _isActive = false;
        _currentOverworldZone = null;

        // Unload all loaded chunks
        var chunksToUnload = new List<string>(_loadedChunkIds);
        var tasks = new List<Task>();

        foreach (var chunkId in chunksToUnload)
        {
            var chunk = FindChunkById(chunkId);
            if (chunk != null)
            {
                tasks.Add(ZoneLoader.Instance.UnloadSceneAsync(chunk.sceneAddress));
            }
        }

        await Task.WhenAll(tasks);
        _loadedChunkIds.Clear();

        LogManager.Trace("[ChunkStreamingManager] Stopped streaming, all chunks unloaded.");
    }

    private void Update()
    {
        if (!_isActive || _currentOverworldZone == null) return;

        _updateTimer += Time.deltaTime;
        if (_updateTimer >= updateInterval)
        {
            _updateTimer = 0f;
            _ = UpdateChunksAroundPlayer();
        }
    }

    private async Task UpdateChunksAroundPlayer()
    {
        if (_isUpdating) return;
        _isUpdating = true;

        try
        {
            var player = WorldManager.Instance.Player;
            if (player == null) return;

            // Calculate which chunk the player is in
            Vector2Int currentChunkCoord = new Vector2Int(
                Mathf.FloorToInt(player.transform.position.x / WorldConstants.CHUNK_SIZE),
                Mathf.FloorToInt(player.transform.position.y / WorldConstants.CHUNK_SIZE)
            );

            if (currentChunkCoord == _lastPlayerChunkCoord) return;
            _lastPlayerChunkCoord = currentChunkCoord;

            int radius = _currentOverworldZone.chunkLoadRadius;

            // Build set of chunks that SHOULD be loaded
            HashSet<string> desiredChunks = new HashSet<string>();

            foreach (var chunk in _currentOverworldZone.chunks)
            {
                int dx = Mathf.Abs(chunk.chunkCoord.x - currentChunkCoord.x);
                int dy = Mathf.Abs(chunk.chunkCoord.y - currentChunkCoord.y);

                if (dx <= radius && dy <= radius)
                {
                    desiredChunks.Add(chunk.chunkId);
                }
            }

            // Unload chunks no longer needed
            var chunksToUnload = _loadedChunkIds.Except(desiredChunks).ToList();
            var chunksToLoad = desiredChunks.Except(_loadedChunkIds).ToList();

            var tasks = new List<Task>();

            foreach (var chunkId in chunksToUnload)
            {
                var chunk = FindChunkById(chunkId);
                if (chunk != null)
                {
                    _loadedChunkIds.Remove(chunkId);
                    tasks.Add(ZoneLoader.Instance.UnloadSceneAsync(chunk.sceneAddress));
                }
            }

            foreach (var chunkId in chunksToLoad)
            {
                var chunk = FindChunkById(chunkId);
                if (chunk != null)
                {
                    _loadedChunkIds.Add(chunkId);
                    tasks.Add(ZoneLoader.Instance.LoadSceneAsync(chunk.sceneAddress));
                }
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <summary>
    /// Determine which chunk coord a world position falls into.
    /// </summary>
    private Vector2Int GetChunkCoordForPosition(Vector3 worldPos)
    {
        // Direct math — no searching needed
        // Works for negative positions automatically
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / WorldConstants.CHUNK_SIZE),
            Mathf.FloorToInt(worldPos.y / WorldConstants.CHUNK_SIZE)
        );
    }

    private ChunkDefinition FindChunkById(string chunkId)
    {
        if (_currentOverworldZone == null) return null;
        return _currentOverworldZone.chunks.FirstOrDefault(c => c.chunkId == chunkId);
    }

    /// <summary>
    /// Force load a specific chunk (e.g., for spawning).
    /// </summary>
    public async Task ForceLoadChunkAt(Vector3 worldPos)
    {
        if (_currentOverworldZone == null) return;

        foreach (var chunk in _currentOverworldZone.chunks)
        {
            if (chunk.GetWorldBounds().Contains(worldPos))
            {
                if (!_loadedChunkIds.Contains(chunk.chunkId))
                {
                    _loadedChunkIds.Add(chunk.chunkId);
                    await ZoneLoader.Instance.LoadSceneAsync(chunk.sceneAddress);
                }
                return;
            }
        }
    }
}
