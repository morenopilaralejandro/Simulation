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

    [SerializeField] private SceneGroup sceneBattle;

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

            BattleArgs.SetMini(
                "faith_selection", 
                encounterData.teamId);
            SceneLoader.Instance.LoadGroup(sceneBattle);

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

    /// <summary>
    /// Called by the encounter system when combat ends.
    /// Reloads the zone the player was in and restores control.
    /// </summary>
    public async void ReturnFromEncounter(string zoneId, string spawnId)
    {
        if (_isTransitioning)
        {
            LogManager.Warning("[WorldManager] Transition already in progress, ignoring return.");
            return;
        }

        _isTransitioning = true;

        try
        {
            var targetZone = FindZone(zoneId);
            if (targetZone == null)
            {
                LogManager.Error($"[WorldManager] Cannot return to zone '{zoneId}' — not found!");
                return;
            }

            await LoadZone(targetZone, spawnId);

            if (transitionScreen != null)
            {
                await transitionScreen.FadeIn();
            }

            player.SetControlEnabled(true);
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[WorldManager] Exception returning from encounter: {e.Message}\n{e.StackTrace}");
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

    #region Helpers

    #endregion
}
