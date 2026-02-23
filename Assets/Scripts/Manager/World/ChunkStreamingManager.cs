using UnityEngine;
using System.Collections.Generic;
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
    private Vector2Int _lastPlayerChunkCoord;
    private float _updateTimer;
    private bool _isActive;
    private bool _isUpdating;

    // Cached 1/CHUNK_SIZE to replace divisions with multiplications
    private float _invChunkSize;

    // Track which chunks are currently loaded
    private readonly HashSet<string> _loadedChunkIds = new HashSet<string>();

    // O(1) chunk lookup built once at StartStreaming
    private readonly Dictionary<string, ChunkDefinition> _chunkLookup
        = new Dictionary<string, ChunkDefinition>();

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

        // Build O(1) lookup table once
        _chunkLookup.Clear();

        List<ChunkDefinition> chunks = zone.chunks;
        for (int i = 0, len = chunks.Count; i < len; i++)
        {
            _chunkLookup[chunks[i].chunkId] = chunks[i];
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogManager.Trace(string.Concat("[ChunkStreamingManager] Started streaming for zone: ", zone.zoneName));
#endif

        // Force immediate update
        UpdateChunksAroundPlayerFireAndForget();
    }

    /// <summary>
    /// Stop streaming and unload all chunks sequentially to limit memory spikes.
    /// </summary>
    public async Task StopStreaming()
    {
        _isActive = false;
        _currentOverworldZone = null;

        // Copy into reusable list to iterate safely
        _tempUnloadAll.Clear();
        foreach (string chunkId in _loadedChunkIds)
        {
            _tempUnloadAll.Add(chunkId);
        }

        // Sequential unloading — safer on low-end devices
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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogManager.Trace("[ChunkStreamingManager] Stopped streaming, all chunks unloaded.");
#endif
    }

    private void Update()
    {
        if (!_isActive || _currentOverworldZone == null) return;

        _updateTimer += Time.deltaTime;
        if (_updateTimer >= updateInterval)
        {
            _updateTimer = 0f;
            UpdateChunksAroundPlayerFireAndForget();
        }
    }

    /// <summary>
    /// Wraps the async update with basic error logging so fire-and-forget
    /// doesn't silently swallow exceptions.
    /// </summary>
    private async void UpdateChunksAroundPlayerFireAndForget()
    {
        try
        {
            await UpdateChunksAroundPlayer();
        }
        catch (System.Exception e)
        {
            LogManager.Error(string.Concat("[ChunkStreamingManager] Chunk update exception: ", e.Message));
        }
    }

    private async Task UpdateChunksAroundPlayer()
    {
        if (_isUpdating) return;
        _isUpdating = true;

        try
        {
            Transform player = WorldManager.Instance.Player.transform;
            if (player == null) return;

            // Calculate which chunk the player is in (multiply instead of divide)
            Vector3 pos = player.transform.position;
            int cx = Mathf.FloorToInt(pos.x * _invChunkSize);
            int cy = Mathf.FloorToInt(pos.y * _invChunkSize);

            if (cx == _lastPlayerChunkCoord.x && cy == _lastPlayerChunkCoord.y) return;
            _lastPlayerChunkCoord.x = cx;
            _lastPlayerChunkCoord.y = cy;

            int radius = _currentOverworldZone.chunkLoadRadius;
            List<ChunkDefinition> chunks = _currentOverworldZone.chunks;

            // Build set of chunks that SHOULD be loaded (reuse set)
            _desiredChunks.Clear();
            for (int i = 0, len = chunks.Count; i < len; i++)
            {
                ChunkDefinition chunk = chunks[i];
                int dx = chunk.chunkCoord.x - cx;
                int dy = chunk.chunkCoord.y - cy;

                // Branchless abs via ternary; avoids Mathf.Abs call overhead
                if (dx < 0) dx = -dx;
                if (dy < 0) dy = -dy;

                if (dx <= radius && dy <= radius)
                {
                    _desiredChunks.Add(chunk.chunkId);
                }
            }

            // Determine which to unload (loaded but not desired)
            _chunksToUnload.Clear();
            foreach (string loadedId in _loadedChunkIds)
            {
                if (!_desiredChunks.Contains(loadedId))
                {
                    _chunksToUnload.Add(loadedId);
                }
            }

            // Determine which to load (desired but not loaded)
            _chunksToLoad.Clear();
            foreach (string desiredId in _desiredChunks)
            {
                if (!_loadedChunkIds.Contains(desiredId))
                {
                    _chunksToLoad.Add(desiredId);
                }
            }

            // Unload first to free memory before loading new chunks
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

            // Then load new chunks sequentially to limit memory spikes
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
    /// Force load a specific chunk (e.g., for spawning).
    /// </summary>
    public async Task ForceLoadChunkAt(Vector3 worldPos)
    {
        if (_currentOverworldZone == null) return;

        List<ChunkDefinition> chunks = _currentOverworldZone.chunks;
        for (int i = 0, len = chunks.Count; i < len; i++)
        {
            ChunkDefinition chunk = chunks[i];
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
