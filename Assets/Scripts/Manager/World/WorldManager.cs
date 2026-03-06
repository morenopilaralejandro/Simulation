using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.World;

/// <summary>
/// Top-level manager that lives in the World scene.
/// Orchestrates zone loading, transitions, and player placement.
/// </summary>
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    private PlayerWorldEntity player;
    public PlayerWorldEntity PlayerWorldEntity => player;
    private TransitionScreen transitionScreen => WorldUIManager.Instance.TransitionScreen;

    [SerializeField] private OverworldDefinition overworldDefinition;
    public OverworldDefinition OverworldDefinition => overworldDefinition;

    private WorldState _currentState = WorldState.None;
    public WorldState CurrentState => _currentState;

    private ZoneDefinition _currentZone;
    public ZoneDefinition CurrentZone => _currentZone;

    private string _pendingSpawnId;

    // Guard against re-entrant transitions
    private bool _isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        WorldEvents.OnEncounterTriggered += HandleEncounterTriggered;
    }

    private void OnDisable()
    {
        WorldEvents.OnEncounterTriggered -= HandleEncounterTriggered;
    }

    private async void Start()
    {
        // Disable player control during initial load
        player = WorldManagerPlayer.Instance.PlayerWorldEntity;
        player.SetControlEnabled(false);

        if(WorldArgs.WorldState == WorldState.InEncounter) 
        {
            ReturnFromEncounter();
            return;
        }

        if (overworldDefinition.startingZone != null)
        {
            await LoadZone(overworldDefinition.startingZone, overworldDefinition.startingSpawnId);
            player.SetControlEnabled(true);
        }
        else
        {
            LogManager.Error("[WorldManager] No starting zone assigned!");
        }
    }

    // =========================================================
    // Encounter Handling
    // =========================================================

    /// <summary>
    /// Called when an encounter is triggered. Disables player control,
    /// unloads all zones, stops chunk streaming, and transitions
    /// the world state to InEncounter.
    /// </summary>
    private async void HandleEncounterTriggered(EncounterData encounterData)
    {
        if (_isTransitioning)
        {
            LogManager.Warning("[WorldManager] Encounter triggered during transition, ignoring.");
            return;
        }

        if (_currentState == WorldState.InEncounter)
        {
            LogManager.Warning("[WorldManager] Already in encounter, ignoring.");
            return;
        }

        _isTransitioning = true;

        LogManager.Trace($"[WorldManager] Encounter triggered: {encounterData}. Tearing down world zones.");

        // Immediately freeze the player
        player.SetControlEnabled(false);

        try
        {
            WorldArgs.Set(
                zoneId : _currentZone != null ? _currentZone.zoneId : null,
                playerPosition : player.transform.position,
                facingDirection : player.FacingToVector(player.FacingDirection),
                worldState : WorldState.InEncounter
            );

            // Fade out so the player doesn't see scenes disappearing
            if (transitionScreen != null)
            {
                await transitionScreen.FadeOut();
            }

            // Stop chunk streaming first (prevents new chunks from loading
            // while we're tearing things down)
            if (_currentZone != null && _currentZone.zoneType == ZoneType.Overworld)
            {
                await ChunkStreamingManager.Instance.StopStreaming();
            }

            // Unload whatever zone is currently loaded
            bool unloadSuccess = await UnloadCurrentZone();
            if (!unloadSuccess)
            {
                LogManager.Warning("[WorldManager] Zone unload reported failure during encounter transition.");
            }

            _currentState = WorldState.InEncounter;

            LogManager.Trace("[WorldManager] World zones unloaded. Ready for encounter scene.");

            WorldManagerEncounter.Instance.StartEncounterBattle(encounterData);

            // NOTE: At this point the encounter system (e.g. EncounterManager)
            // should take over — load its own scene, run combat, etc.
            // When the encounter ends(results), it should call WorldManager.ReturnFromEncounter()
            // to reload the zone the player was in.
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[WorldManager] Exception during encounter teardown: {e.Message}\n{e.StackTrace}");

            // Attempt recovery so the game isn't permanently stuck
            _currentState = WorldState.None;
            if (transitionScreen != null)
            {
                await transitionScreen.FadeIn();
            }
            player.SetControlEnabled(true);
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    public async void ReturnFromEncounter()
    {
        _isTransitioning = true;

        try
        {
            var targetZone = FindZone(WorldArgs.ZoneId);
            if (targetZone == null)
            {
                LogManager.Error($"[WorldManager] Cannot return to zone '{WorldArgs.ZoneId}'");
                return;
            }

            // Load the zone but place the player at the SAVED position
            await LoadZoneAtPosition(targetZone, WorldArgs.PlayerPosition, WorldArgs.FacingDirection);

            if (transitionScreen != null)
                await transitionScreen.FadeIn();

            player.SetControlEnabled(true);
            player.SetState(PlayerWorldState.FreeRoam);
            WorldManagerEncounter.Instance.ResetStepCounter();
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[WorldManager] Exception returning from encounter: {e.Message}\n{e.StackTrace}");
            player.SetControlEnabled(true);
            if (transitionScreen != null) await transitionScreen.FadeIn();
        }
        finally
        {
            _isTransitioning = false;
        }
    }

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

    private async Task LoadZone(ZoneDefinition zone, string spawnId)
    {
        _currentState = WorldState.Loading;
        _currentZone = zone;

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
        ZoneTracker.Instance.SetZone(zone);
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
    private async Task<bool> UnloadCurrentZone()
    {
        if (_currentZone == null)
        {
            return true;
        }

        ZoneDefinition zoneToUnload = _currentZone;

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

        _currentZone = null;
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

    private async Task LoadZoneAtPosition(ZoneDefinition zone, Vector3 position, Vector2 facing)
    {
        _currentState = WorldState.Loading;
        _currentZone = zone;

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
        ZoneTracker.Instance.SetZone(zone);
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
