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

    [Header("Zone Database")]
    public List<ZoneDefinition> allZones = new List<ZoneDefinition>();

    [Header("Starting Zone")]
    public ZoneDefinition startingZone;
    public string startingSpawnId;

    [Header("State")]
    [SerializeField] private WorldState _currentState = WorldState.None;
    public WorldState CurrentState => _currentState;

    private ZoneDefinition _currentZone;
    public ZoneDefinition CurrentZone => _currentZone;

    private string _pendingSpawnId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private async void Start()
    {
        // Disable player control during initial load
        player = WorldManagerPlayer.Instance.PlayerWorldEntity;
        player.SetControlEnabled(false);

        if (startingZone != null)
        {
            await LoadZone(startingZone, startingSpawnId);
            player.SetControlEnabled(true);
        }
        else
        {
            LogManager.Error("[WorldManager] No starting zone assigned!");
        }
    }

    /// <summary>
    /// Public entry point for zone transitions (called by triggers, doors, etc.)
    /// </summary>
    public async void TransitionToZone(string zoneId, string spawnId)
    {
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

        _currentState = WorldState.Transitioning;
        player.SetControlEnabled(false);

        // Fade out
        if (transitionScreen != null)
        {
            await transitionScreen.FadeOut();
        }

        // Unload current zone
        await UnloadCurrentZone();

        // Load new zone
        await LoadZone(targetZone, spawnId);

        // Fade in
        if (transitionScreen != null)
        {
            await transitionScreen.FadeIn();
        }

        player.SetControlEnabled(true);
    }

    private async Task LoadZone(ZoneDefinition zone, string spawnId)
    {
        _currentState = WorldState.Loading;
        _currentZone = zone;

        LogManager.Trace($"[WorldManager] Loading zone: {zone.zoneName} ({zone.zoneType})");

        if (zone.zoneType == ZoneType.Overworld)
        {
            // Loads the right chunk, places player, starts streaming
            await LoadOverworldZone(zone, spawnId);
        }
        else if (zone.zoneType == ZoneType.Interior)
        {
            // Loads single scene, places player
            await LoadInteriorZone(zone, spawnId);
        }

        if (zone.backgroundMusic != null)
        {
            AudioManager.Instance.PlayBgmClip(zone.backgroundMusic);
        }
    }

    private async Task LoadOverworldZone(ZoneDefinition zone, string spawnId)
    {
        // =====================================================
        // STEP 1: Find which chunk contains our target spawn
        // =====================================================
        // 
        // The player needs to appear at a specific spawn point
        // (e.g., "house_01_exit" after leaving a house).
        // That spawn point is a GameObject inside one specific 
        // chunk scene. We need to figure out WHICH chunk scene
        // before we can load anything.
        
        ChunkDefinition spawnChunk = null;

        // Search through all chunks in this zone to find which
        // one has our target spawn ID
        foreach (var chunk in zone.chunks)
        {
            if (chunk.containedSpawnIds.Contains(spawnId))
            {
                spawnChunk = chunk;
                break;
            }
        }

        // Fallback: if spawn not found, use the chunk at (0,0)
        if (spawnChunk == null)
        {
            LogManager.Warning(
                $"[WorldManager] No chunk claims spawn '{spawnId}'. " +
                $"Falling back to first chunk."
            );
            spawnChunk = zone.chunks[0];
        }

        // =====================================================
        // STEP 2: Load the spawn chunk FIRST and wait for it
        // =====================================================
        //
        // We MUST load this chunk before anything else because
        // the SpawnPoint component is inside this scene.
        // When the scene loads, ChunkSceneRoot.Start() runs
        // and registers all its SpawnPoints with the registry.

        await ZoneLoader.Instance.LoadSceneAsync(spawnChunk.sceneAddress);

        // Wait two frames for Unity to initialize the scene
        // (Awake runs on load, Start runs next frame)
        await Task.Yield(); // frame 1: Awake
        await Task.Yield(); // frame 2: Start (SpawnPoints register)

        // =====================================================
        // STEP 3: Find the spawn point and place the player
        // =====================================================
        //
        // Now the chunk scene is loaded and its SpawnPoint 
        // components have registered themselves. We can look 
        // up the actual world position.

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
            // Even this shouldn't happen now, but just in case
            LogManager.Warning($"[WorldManager] Spawn '{spawnId}' not found after loading chunk!");
            player.Teleport(spawnChunk.GetWorldBounds().center);
        }

        // =====================================================
        // STEP 4: Start chunk streaming from player's position
        // =====================================================
        //
        // Now the player is positioned correctly. We start the
        // streaming system which will:
        //   - See the player is at chunk (x,y)
        //   - Load all chunks within chunkLoadRadius
        //   - The spawn chunk is already loaded, so it won't
        //     double-load it (ZoneLoader checks for duplicates)
        //   - Neighboring chunks load in the background

        ChunkStreamingManager.Instance.StartStreaming(zone);

        _currentState = WorldState.InOverworld;
    }

    private async Task LoadInteriorZone(ZoneDefinition zone, string spawnId)
    {
        // STEP 1: Load the single interior scene
        await ZoneLoader.Instance.LoadSceneAsync(zone.interiorSceneAddress);
        
        await Task.Yield();
        await Task.Yield();

        // STEP 2: Find spawn and place player
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

        // No chunk streaming for interiors — the whole scene is loaded
        _currentState = WorldState.InInterior;
    }

    private async Task UnloadCurrentZone()
    {
        if (_currentZone == null) return;

        SpawnPointRegistry.Instance.Clear();

        if (_currentZone.zoneType == ZoneType.Overworld)
        {
            await ChunkStreamingManager.Instance.StopStreaming();
        }
        else if (_currentZone.zoneType == ZoneType.Interior)
        {
            await ZoneLoader.Instance.UnloadSceneAsync(_currentZone.interiorSceneAddress);
        }

        _currentZone = null;
    }

    private async Task PlacePlayerAtSpawn(string zoneId, string spawnId)
    {
        // Wait for spawn points to register (scenes just loaded)
        int maxRetries = 20;
        SpawnPoint spawn = null;

        for (int i = 0; i < maxRetries; i++)
        {
            spawn = SpawnPointRegistry.Instance.FindSpawnPoint(zoneId, spawnId);

            if (spawn == null && !string.IsNullOrEmpty(spawnId))
            {
                // Also search without zone prefix in case of chunk scenes
                // that register under the zone id
                spawn = SpawnPointRegistry.Instance.FindSpawnPoint(zoneId, spawnId);
            }

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

            // Try default spawn
            var defaultSpawn = SpawnPointRegistry.Instance.FindSpawnPointByType(zoneId, SpawnPointType.Default);
            if (defaultSpawn != null)
            {
                player.Teleport(spawn.GetSpawnPosition());
            }
        }
    }

    public ZoneDefinition FindZone(string zoneId)
    {
        return allZones.Find(z => z.zoneId == zoneId);
    }

    #region Helpers

    #endregion
}
