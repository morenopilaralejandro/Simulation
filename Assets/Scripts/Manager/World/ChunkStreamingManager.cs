using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Manages streaming of overworld chunks based on player position.
/// Chunks can belong to ANY zone — the overworld is a seamless collection
/// of chunks across zone boundaries.
/// </summary>
public class ChunkStreamingManager : MonoBehaviour
{
    public static ChunkStreamingManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("How often to check player position for chunk updates (seconds)")]
    [SerializeField] private float updateInterval = 1f;

    private OverworldDefinition _overworld;
    private Vector2Int _lastPlayerChunkCoord;
    private float _updateTimer;
    private bool _isActive;
    private bool _isUpdating;

    private float _invChunkSize;
    private int _chunkLoadRadius;

    // Track which chunks are currently loaded
    private readonly HashSet<string> _loadedChunkIds = new HashSet<string>();

    // O(1) chunk lookup built once at StartStreaming
    private readonly Dictionary<string, ChunkDefinition> _chunkLookup
        = new Dictionary<string, ChunkDefinition>();

    // Spatial lookup: chunk coord → chunk (for fast radius queries)
    private readonly Dictionary<Vector2Int, ChunkDefinition> _coordLookup
        = new Dictionary<Vector2Int, ChunkDefinition>();

    // --- Reusable collections to avoid per-update allocations ---
    private readonly HashSet<string> _desiredChunks = new HashSet<string>();
    private readonly List<string> _chunksToUnload = new List<string>();
    private readonly List<string> _chunksToLoad = new List<string>();
    private readonly List<string> _tempUnloadAll = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _lastPlayerChunkCoord = new Vector2Int(int.MinValue, int.MinValue);
        _invChunkSize = 1f / WorldConstants.CHUNK_SIZE;
    }

    /// <summary>
    /// Start streaming chunks for the entire overworld.
    /// All zones' chunks are available for streaming.
    /// </summary>
    public void StartStreaming(OverworldDefinition overworld)
    {
        if (overworld == null)
        {
            LogManager.Error("[ChunkStreamingManager] OverworldDefinition is null!");
            return;
        }

        _overworld = overworld;
        _chunkLoadRadius = overworld.chunkLoadRadius;
        _lastPlayerChunkCoord = new Vector2Int(int.MinValue, int.MinValue);
        _isActive = true;
        _updateTimer = 0f;

        // Build O(1) lookup tables once
        _chunkLookup.Clear();
        _coordLookup.Clear();

        List<ChunkDefinition> chunks = overworld.allChunks;
        for (int i = 0, len = chunks.Count; i < len; i++)
        {
            ChunkDefinition chunk = chunks[i];
            _chunkLookup[chunk.chunkId] = chunk;
            _coordLookup[chunk.chunkCoord] = chunk;
        }

        // ============================================================
        // Seed _loadedChunkIds with chunks that ZoneLoader already has
        // loaded (e.g., from PreloadChunksAroundPosition).
        // This prevents the streaming manager from ignoring them during
        // unload decisions or trying to double-load them.
        // ============================================================
        _loadedChunkIds.Clear();
        for (int i = 0, len = chunks.Count; i < len; i++)
        {
            ChunkDefinition chunk = chunks[i];
            if (ZoneLoader.Instance.IsSceneLoaded(chunk.sceneAddress))
            {
                _loadedChunkIds.Add(chunk.chunkId);
            }
        }

        // Initialize zone tracking
        ZoneTracker.Instance.Initialize(overworld);

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogManager.Trace(string.Concat(
            "[ChunkStreamingManager] Started streaming overworld with ",
            chunks.Count.ToString(), " chunks (",
            _loadedChunkIds.Count.ToString(), " pre-loaded) across ",
            overworld.allZones.Count.ToString(), " zones."));
    #endif

        // Force immediate update
        UpdateChunksAroundPlayerFireAndForget();
    }

    /// <summary>
    /// Stop streaming and unload all chunks sequentially.
    /// </summary>
    public async Task StopStreaming()
    {
        _isActive = false;
        _overworld = null;

        _tempUnloadAll.Clear();
        foreach (string chunkId in _loadedChunkIds)
        {
            _tempUnloadAll.Add(chunkId);
        }

        for (int i = 0, count = _tempUnloadAll.Count; i < count; i++)
        {
            ChunkDefinition chunk;
            if (_chunkLookup.TryGetValue(_tempUnloadAll[i], out chunk))
            {
                await ZoneLoader.Instance.UnloadSceneAsync(chunk.sceneAddress);
            }
        }

        _loadedChunkIds.Clear();
        _tempUnloadAll.Clear();
        _chunkLookup.Clear();
        _coordLookup.Clear();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogManager.Trace("[ChunkStreamingManager] Stopped streaming, all chunks unloaded.");
#endif
    }

    private void Update()
    {
        if (!_isActive || _overworld == null) return;

        _updateTimer += Time.deltaTime;
        if (_updateTimer >= updateInterval)
        {
            _updateTimer = 0f;
            UpdateChunksAroundPlayerFireAndForget();
        }
    }

    private async void UpdateChunksAroundPlayerFireAndForget()
    {
        try
        {
            await UpdateChunksAroundPlayer();
        }
        catch (System.Exception e)
        {
            LogManager.Error(string.Concat(
                "[ChunkStreamingManager] Chunk update exception: ", e.Message));
        }
    }

    private async Task UpdateChunksAroundPlayer()
    {
        if (_isUpdating) return;
        _isUpdating = true;

        try
        {
            Transform player = WorldManager.Instance.PlayerWorldEntity.transform;
            if (player == null) return;

            Vector3 pos = player.position;
            int cx = Mathf.FloorToInt(pos.x * _invChunkSize);
            int cy = Mathf.FloorToInt(pos.y * _invChunkSize);

            if (cx == _lastPlayerChunkCoord.x && cy == _lastPlayerChunkCoord.y) return;
            _lastPlayerChunkCoord.x = cx;
            _lastPlayerChunkCoord.y = cy;

            // Notify zone tracker of player's new position
            ZoneTracker.Instance.UpdatePlayerPosition(pos);

            int radius = _chunkLoadRadius;

            // Build set of desired chunks using spatial lookup (O(radius²) instead of O(n))
            _desiredChunks.Clear();
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2Int coord = new Vector2Int(cx + dx, cy + dy);
                    ChunkDefinition chunk;
                    if (_coordLookup.TryGetValue(coord, out chunk))
                    {
                        _desiredChunks.Add(chunk.chunkId);
                    }
                }
            }

            // Determine which to unload
            _chunksToUnload.Clear();
            foreach (string loadedId in _loadedChunkIds)
            {
                if (!_desiredChunks.Contains(loadedId))
                {
                    _chunksToUnload.Add(loadedId);
                }
            }

            // Determine which to load
            _chunksToLoad.Clear();
            foreach (string desiredId in _desiredChunks)
            {
                if (!_loadedChunkIds.Contains(desiredId))
                {
                    _chunksToLoad.Add(desiredId);
                }
            }

            // Unload first to free memory
            for (int i = 0, count = _chunksToUnload.Count; i < count; i++)
            {
                string chunkId = _chunksToUnload[i];
                _loadedChunkIds.Remove(chunkId);

                ChunkDefinition chunk;
                if (_chunkLookup.TryGetValue(chunkId, out chunk))
                {
                    await ZoneLoader.Instance.UnloadSceneAsync(chunk.sceneAddress);
                }
            }

            // Then load new chunks
            for (int i = 0, count = _chunksToLoad.Count; i < count; i++)
            {
                string chunkId = _chunksToLoad[i];
                _loadedChunkIds.Add(chunkId);

                ChunkDefinition chunk;
                if (_chunkLookup.TryGetValue(chunkId, out chunk))
                {
                    await ZoneLoader.Instance.LoadSceneAsync(chunk.sceneAddress);
                }
            }
        }
        finally
        {
            _isUpdating = false;
        }
    }

    /// <summary>
    /// Force load a specific chunk at a world position (e.g., for spawning).
    /// Works across any zone's chunks.
    /// </summary>
    public async Task ForceLoadChunkAt(Vector3 worldPos)
    {
        if (_overworld == null) return;

        int cx = Mathf.FloorToInt(worldPos.x * _invChunkSize);
        int cy = Mathf.FloorToInt(worldPos.y * _invChunkSize);
        Vector2Int coord = new Vector2Int(cx, cy);

        ChunkDefinition chunk;
        if (_coordLookup.TryGetValue(coord, out chunk))
        {
            if (!_loadedChunkIds.Contains(chunk.chunkId))
            {
                _loadedChunkIds.Add(chunk.chunkId);
                await ZoneLoader.Instance.LoadSceneAsync(chunk.sceneAddress);
            }
        }
    }

    /// <summary>
    /// Force load the chunk at a spawn point's position and return the spawn data.
    /// </summary>
    /*
    public async Task<SpawnPointDefinition> LoadAndResolveSpawn(
        string zoneId, string spawnId = null)
    {
        SpawnPointDefinition spawn = ZoneTracker.Instance.ResolveSpawnPoint(zoneId, spawnId);
        if (spawn == null)
        {
            LogManager.Error(string.Concat(
                "[ChunkStreamingManager] Could not resolve spawn in zone: ", zoneId));
            return null;
        }

        await ForceLoadChunkAt(spawn.worldPosition);
        return spawn;
    }
    */

    /// <summary>
    /// Get the zone the player is currently in (delegates to ZoneTracker).
    /// </summary>
    public ZoneDefinition GetCurrentPlayerZone()
    {
        return ZoneTracker.Instance != null ? ZoneTracker.Instance.CurrentZone : null;
    }
}
