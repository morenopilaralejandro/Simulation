using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.World;

public class WorldManagerZone
{
    private WorldManager worldManager;

    private OverworldDefinition overworldDefinition;

    private WorldState _currentState = WorldState.None;
    public WorldState CurrentState => _currentState;
    public void SetState(WorldState state) => _currentState = state;

    private TransitionScreen transitionScreen => WorldUIManager.Instance.TransitionScreen;
    public Task FadeInAsync() => transitionScreen.FadeIn();
    public Task FadeOutAsync() => transitionScreen.FadeOut();
    private PlayerWorldEntity player;
    private bool _isTransitioning = false;
    public bool IsTransitioning => _isTransitioning;
    public void SetIsTransitioning(bool boolValue) => _isTransitioning = boolValue;
    private string _pendingSpawnId;

    public WorldManagerZone(OverworldDefinition overworldDefinition)
    {
        this.overworldDefinition = overworldDefinition;
        worldManager = WorldManager.Instance;

        player = worldManager.PlayerWorldEntity;
        player.SetControlEnabled(false);

        //InitializeAsync();
    }

    //private async void InitializeAsync() { }

    // =========================================================
    // Zone Transitions
    // =========================================================

    /// <summary>
    /// Public entry point for zone transitions (called by triggers, doors, etc.)
    /// </summary>
    public async void TransitionToZone(string zoneId, string spawnId)
    {
        // Guard: prevent overlapping transitions
        if (_isTransitioning)
        {
            LogManager.Warning("[WorldManager] Transition already in progress, ignoring.");
            return;
        }

        if (_currentState == WorldState.Transitioning || _currentState == WorldState.Loading)
        {
            LogManager.Warning("[WorldManager] Already transitioning!");
            return;
        }

        var targetZone = FindZone(zoneId);
        if (targetZone == null)
        {
            LogManager.Error($"[WorldManager] Zone not found: {zoneId}");
            return;
        }

        _isTransitioning = true;
        _currentState = WorldState.Transitioning;
        player.SetControlEnabled(false);

        try
        {
            // Fade out
            if (transitionScreen != null)
            {
                await transitionScreen.FadeOut();
            }

            // Unload current zone — await fully before proceeding
            bool unloadSuccess = await UnloadCurrentZone();
            if (!unloadSuccess)
            {
                LogManager.Warning("[WorldManager] Unload reported failure, proceeding with load anyway.");
            }

            // Load new zone
            await LoadZone(targetZone, spawnId);

            // Fade in
            if (transitionScreen != null)
            {
                await transitionScreen.FadeIn();
            }

            player.SetControlEnabled(true);
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[WorldManager] Exception during transition: {e.Message}\n{e.StackTrace}");

            // Attempt recovery: re-enable player and fade in so the game isn't stuck
            player.SetControlEnabled(true);
            if (transitionScreen != null)
            {
                await transitionScreen.FadeIn();
            }
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    // =========================================================
    // Zone Loading / Unloading
    // =========================================================

    public async Task LoadZone(ZoneDefinition zone, string spawnId)
    {
        _currentState = WorldState.Loading;
        worldManager.SetZone(zone);

        LogManager.Trace($"[WorldManager] Loading zone: {zone.zoneName} ({zone.zoneType})");

        if (zone.zoneType == ZoneType.Overworld)
        {
            await LoadOverworldZone(zone, spawnId);
        }
        else if (zone.zoneType == ZoneType.Interior)
        {
            await LoadInteriorZone(zone, spawnId);
        }

        if (zone.backgroundMusic != null)
        {
            AudioManager.Instance.PlayBgmClip(zone.backgroundMusic);
        }
    }

    private async Task LoadOverworldZone(ZoneDefinition zone, string spawnId)
    {
        ChunkDefinition spawnChunk = null;

        foreach (var chunk in overworldDefinition.allChunks)
        {
            if (chunk.containedSpawnIds.Contains(spawnId))
            {
                spawnChunk = chunk;
                break;
            }
        }

        if (spawnChunk == null)
        {
            LogManager.Warning(
                $"[WorldManager] No chunk claims spawn '{spawnId}'. " +
                $"Falling back to first chunk."
            );
            spawnChunk = overworldDefinition.allChunks[0];
        }

        // Load the spawn chunk first so we can place the player
        await ZoneLoader.Instance.LoadSceneAsync(spawnChunk.sceneAddress);

        await Task.Yield();
        await Task.Yield();

        SpawnPoint spawn = SpawnPointRegistry.Instance.FindSpawnPoint(
            zone.zoneId,
            spawnId
        );

        if (spawn != null)
        {
            player.Teleport(spawn.GetSpawnPosition());
            player.SetFacing(spawn.facingDirection);
            LogManager.Trace($"[WorldManager] Player placed at '{spawnId}' -> {spawn.GetSpawnPosition()}");
        }
        else
        {
            LogManager.Warning($"[WorldManager] Spawn '{spawnId}' not found after loading chunk!");
            player.Teleport(spawnChunk.GetWorldBounds().center);
        }

        // ============================================================
        // PRE-LOAD all chunks within load radius BEFORE starting
        // the streaming manager and BEFORE fading in.
        // This eliminates the pop-in of adjacent chunks.
        // ============================================================
        await PreloadChunksAroundPosition(player.transform.position);

        // Now start the streaming manager — it will see that the chunks
        // are already loaded and won't double-load them.
        ChunkStreamingManager.Instance.StartStreaming(overworldDefinition);

        _currentState = WorldState.InOverworld;
    }

    private async Task LoadInteriorZone(ZoneDefinition zone, string spawnId)
    {
        worldManager.SetZone(zone);
        await ZoneLoader.Instance.LoadSceneAsync(zone.interiorSceneAddress);

        await Task.Yield();
        await Task.Yield();

        SpawnPoint spawn = SpawnPointRegistry.Instance.FindSpawnPoint(
            zone.zoneId,
            spawnId
        );

        if (spawn != null)
        {
            player.Teleport(spawn.GetSpawnPosition());
            player.SetFacing(spawn.facingDirection);
        }
        else
        {
            LogManager.Warning($"[WorldManager] Spawn '{spawnId}' not found in interior '{zone.zoneId}'");
        }

        _currentState = WorldState.InInterior;
    }

    /// <summary>
    /// Unloads the current zone. Returns true if unload succeeded or there was nothing to unload.
    /// </summary>
    public async Task<bool> UnloadCurrentZone()
    {
        if (worldManager.CurrentZone == null)
        {
            return true;
        }

        ZoneDefinition zoneToUnload = worldManager.CurrentZone;

        SpawnPointRegistry.Instance.Clear();

        bool success = true;

        try
        {
            if (zoneToUnload.zoneType == ZoneType.Overworld)
            {
                await ChunkStreamingManager.Instance.StopStreaming();
            }
            else if (zoneToUnload.zoneType == ZoneType.Interior)
            {
                bool unloaded = await ZoneLoader.Instance.UnloadSceneAsync(zoneToUnload.interiorSceneAddress);
                if (!unloaded)
                {
                    LogManager.Warning($"[WorldManager] Failed to unload interior scene: {zoneToUnload.interiorSceneAddress}");
                    success = false;
                }
            }
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[WorldManager] Exception during zone unload: {e.Message}\n{e.StackTrace}");
            success = false;
        }

        worldManager.SetZone(null);
        return success;
    }

    private async Task PlacePlayerAtSpawn(string zoneId, string spawnId)
    {
        int maxRetries = 20;
        SpawnPoint spawn = null;

        for (int i = 0; i < maxRetries; i++)
        {
            spawn = SpawnPointRegistry.Instance.FindSpawnPoint(zoneId, spawnId);

            if (spawn != null) break;

            await Task.Delay(100);
        }

        if (spawn != null)
        {
            player.Teleport(spawn.GetSpawnPosition());
            player.SetFacing(spawn.facingDirection);
            LogManager.Trace($"[WorldManager] Player placed at spawn '{spawnId}' -> {spawn.GetSpawnPosition()}");
        }
        else
        {
            LogManager.Warning($"[WorldManager] Spawn point '{spawnId}' not found in zone '{zoneId}'. Using fallback.");

            var defaultSpawn = SpawnPointRegistry.Instance.FindSpawnPointByType(zoneId, SpawnPointType.Default);
            if (defaultSpawn != null)
            {
                player.Teleport(defaultSpawn.GetSpawnPosition());
            }
        }
    }

    public ZoneDefinition FindZone(string zoneId)
    {
        return overworldDefinition.allZones.Find(z => z.zoneId == zoneId);
    }

    /// <summary>
    /// Loads all chunks within the overworld's load radius around the given
    /// world position. Awaits ALL loads so nothing pops in after fade-in.
    /// </summary>
    private async Task PreloadChunksAroundPosition(Vector3 worldPos)
    {
        if (overworldDefinition == null) return;

        float invChunkSize = 1f / WorldConstants.CHUNK_SIZE;
        int cx = Mathf.FloorToInt(worldPos.x * invChunkSize);
        int cy = Mathf.FloorToInt(worldPos.y * invChunkSize);
        int radius = overworldDefinition.chunkLoadRadius;

        // Build a coord -> chunk lookup (same structure ChunkStreamingManager uses)
        // We build a temporary one here since streaming hasn't started yet.
        Dictionary<Vector2Int, ChunkDefinition> coordLookup 
            = new Dictionary<Vector2Int, ChunkDefinition>();
        
        for (int i = 0; i < overworldDefinition.allChunks.Count; i++)
        {
            ChunkDefinition chunk = overworldDefinition.allChunks[i];
            coordLookup[chunk.chunkCoord] = chunk;
        }

        // Gather all load tasks so they run concurrently
        List<Task> loadTasks = new List<Task>();

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                Vector2Int coord = new Vector2Int(cx + dx, cy + dy);
                ChunkDefinition chunk;
                if (coordLookup.TryGetValue(coord, out chunk))
                {
                    // ZoneLoader.LoadSceneAsync already skips if already loaded
                    loadTasks.Add(ZoneLoader.Instance.LoadSceneAsync(chunk.sceneAddress));
                }
            }
        }

        // Wait for ALL chunks to finish loading
        if (loadTasks.Count > 0)
        {
            LogManager.Trace($"[WorldManager] Pre-loading {loadTasks.Count} chunks around player...");
            await Task.WhenAll(loadTasks);
            LogManager.Trace("[WorldManager] All nearby chunks pre-loaded.");
        }
    }

    // =========================================================
    // New: Load zone and place player at an exact position
    // =========================================================

    public async Task LoadZoneAtPosition(ZoneDefinition zone, Vector3 position, Vector2 facing)
    {
        _currentState = WorldState.Loading;
        worldManager.SetZone(zone);

        LogManager.Trace($"[WorldManager] Loading zone '{zone.zoneName}' at position {position}");

        if (zone.zoneType == ZoneType.Overworld)
        {
            await LoadOverworldZoneAtPosition(zone, position, facing);
        }
        else if (zone.zoneType == ZoneType.Interior)
        {
            await LoadInteriorZoneAtPosition(zone, position, facing);
        }

        if (zone.backgroundMusic != null)
            AudioManager.Instance.PlayBgmClip(zone.backgroundMusic);
    }

    private async Task LoadOverworldZoneAtPosition(ZoneDefinition zone, Vector3 position, Vector2 facing)
    {
        // Find which chunk contains this position
        float invChunkSize = 1f / WorldConstants.CHUNK_SIZE;
        int cx = Mathf.FloorToInt(position.x * invChunkSize);
        int cy = Mathf.FloorToInt(position.y * invChunkSize);
        Vector2Int targetCoord = new Vector2Int(cx, cy);

        ChunkDefinition spawnChunk = null;
        foreach (var chunk in overworldDefinition.allChunks)
        {
            if (chunk.chunkCoord == targetCoord)
            {
                spawnChunk = chunk;
                break;
            }
        }

        if (spawnChunk == null)
        {
            LogManager.Warning($"[WorldManager] No chunk at coord {targetCoord}, falling back to first chunk.");
            spawnChunk = overworldDefinition.allChunks[0];
        }

        // Load the chunk that contains the player's saved position
        await ZoneLoader.Instance.LoadSceneAsync(spawnChunk.sceneAddress);
        await Task.Yield();
        await Task.Yield();

        // Place player at exact saved position
        player.Teleport(position);
        player.SetFacing(facing);

        LogManager.Trace($"[WorldManager] Player restored at {position}");

        // Pre-load surrounding chunks before fade-in
        await PreloadChunksAroundPosition(position);

        // Start streaming
        ChunkStreamingManager.Instance.StartStreaming(overworldDefinition);

        _currentState = WorldState.InOverworld;
    }

    private async Task LoadInteriorZoneAtPosition(ZoneDefinition zone, Vector3 position, Vector2 facing)
    {
        worldManager.SetZone(zone);
        await ZoneLoader.Instance.LoadSceneAsync(zone.interiorSceneAddress);
        await Task.Yield();
        await Task.Yield();

        player.Teleport(position);
        player.SetFacing(facing);

        _currentState = WorldState.InInterior;
    }


    #region Helpers

    #endregion
}
