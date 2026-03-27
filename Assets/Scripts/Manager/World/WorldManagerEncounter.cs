using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Battle;
using Simulation.Enums.World;

public class WorldManagerEncounter
{
    #region Fields

    private SceneGroup sceneBattle;
    private WorldManager worldManager;
    private PlayerWorldEntity player;
    private PlayerWorldConfig config;
    private SceneLoader sceneLoader;
    private ChunkStreamingManager chunkStreamingManager;

    #endregion

    #region Constructor

    public WorldManagerEncounter(SceneGroup sceneBattle)
    {
        this.sceneBattle = sceneBattle;
        worldManager = WorldManager.Instance;
        player = worldManager.PlayerWorldEntity;
        config = worldManager.PlayerWorldConfig;
        sceneLoader = SceneLoader.Instance;
        chunkStreamingManager = ChunkStreamingManager.Instance;

        ResetStepCounter();
    }

    #endregion

    #region Events

    public void Subscribe()
    {
        WorldEvents.OnEncounterTriggered += HandleEncounterTriggered;
        WorldEvents.OnZoneChanged += HandleZoneChanged;
        BattleEvents.OnBattleEnd += HandleBattleEnd;
    }

    public void Unsubscribe()
    {
        WorldEvents.OnEncounterTriggered -= HandleEncounterTriggered;
        WorldEvents.OnZoneChanged -= HandleZoneChanged;
        BattleEvents.OnBattleEnd -= HandleBattleEnd;
    }

    private void HandleBattleEnd() 
    {
        //Resume overworld
        //playerWorldEntity.SetState(PlayerWorldState.FreeRoam);
        //WorldManager.ReturnFromEncounter()
    }

    private void HandleZoneChanged(ZoneDefinition previousZone, ZoneDefinition newZone, string newName) 
    {
        if (previousZone == newZone) return;
        ResetStepCounter();
    }

    /// <summary>
    /// Called when an encounter is triggered. Disables player control,
    /// unloads all zones, stops chunk streaming, and transitions
    /// the world state to InEncounter.
    /// </summary>
    private async void HandleEncounterTriggered(EncounterData encounterData)
    {
        if (worldManager.IsTransitioning)
        {
            LogManager.Warning("[WorldManager] Encounter triggered during transition, ignoring.");
            return;
        }

        if (worldManager.CurrentState == WorldState.InEncounter)
        {
            LogManager.Warning("[WorldManager] Already in encounter, ignoring.");
            return;
        }

        worldManager.SetIsTransitioning(true);

        LogManager.Trace($"[WorldManager] Encounter triggered: {encounterData}. Tearing down world zones.");

        // Immediately freeze the player
        player.SetControlEnabled(false);

        try
        {
            WorldArgs.Set(
                zoneId : worldManager.CurrentZone != null ? worldManager.CurrentZone.zoneId : null,
                realm : worldManager.CurrentRealm,
                playerPosition : player.CurrentTilePosition3d(),
                facingDirection : player.FacingToVector(player.FacingDirection),
                worldState : WorldState.InEncounter
            );

            // Fade out so the player doesn't see scenes disappearing

                await worldManager.FadeOut();

            // Stop chunk streaming first (prevents new chunks from loading
            // while we're tearing things down)
            if (worldManager.CurrentZone != null && worldManager.CurrentZone.zoneType == ZoneType.Overworld)
            {
                await chunkStreamingManager.StopStreaming();
            }

            // Unload whatever zone is currently loaded
            bool unloadSuccess = await worldManager.UnloadCurrentZone();
            if (!unloadSuccess)
            {
                LogManager.Warning("[WorldManager] Zone unload reported failure during encounter transition.");
            }

            worldManager.SetState(WorldState.InEncounter);

            LogManager.Trace("[WorldManager] World zones unloaded. Ready for encounter scene.");

            StartEncounterBattle(encounterData);

            // NOTE: At this point the encounter system (e.g. EncounterManager)
            // should take over — load its own scene, run combat, etc.
            // When the encounter ends(results), it should call WorldManager.ReturnFromEncounter()
            // to reload the zone the player was in.
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[WorldManager] Exception during encounter teardown: {e.Message}\n{e.StackTrace}");

            // Attempt recovery so the game isn't permanently stuck
            worldManager.SetState(WorldState.None);

                await worldManager.FadeIn();
    
            player.SetControlEnabled(true);
        }
        finally
        {
            worldManager.SetIsTransitioning(false);
        }
    }

    public async void ReturnFromEncounter()
    {
        worldManager.SetIsTransitioning(true);

        try
        {
            var targetZone = worldManager.FindZone(WorldArgs.ZoneId);
            if (targetZone == null)
            {
                LogManager.Error($"[WorldManager] Cannot return to zone '{WorldArgs.ZoneId}'");
                return;
            }

            // Load the zone but place the player at the SAVED position
            await worldManager.LoadZoneAtPosition(targetZone, WorldArgs.PlayerPosition, WorldArgs.FacingDirection);

                await worldManager.FadeIn();

            player.SetControlEnabled(true);
            player.SetState(PlayerWorldState.FreeRoam);
            ResetStepCounter();
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[WorldManager] Exception returning from encounter: {e.Message}\n{e.StackTrace}");
            player.SetControlEnabled(true);
            await worldManager.FadeIn();
        }
        finally
        {
            worldManager.SetIsTransitioning(false);
        }
    }

    #endregion

    #region Logic

    private int _stepsUntilEncounter;
    private float _distanceAccumulator;
    private const float STEP_DISTANCE = 1f;

    /// <summary>Called from PlayerWorldComponentController.</summary>
    public void Tick(bool isMoving, float speed, float deltaTime)
    {
        if (!isMoving || worldManager.CurrentZone == null) return;

        _distanceAccumulator += speed * deltaTime;

        while (_distanceAccumulator >= STEP_DISTANCE)
        {
            _distanceAccumulator -= STEP_DISTANCE;
            _stepsUntilEncounter--;

            if (_stepsUntilEncounter <= 0)
            {
                TryTriggerEncounter();
                ResetStepCounter();
                return;
            }
        }
    }

    /// <summary>
    /// Called once each time the player arrives on a new grid tile.
    /// </summary>
    public void OnTileArrived(bool isRunning)
    {
        if (worldManager.CurrentZone == null) return;

        int stepValue = isRunning ? config.runStepMultiplier : 1;
        _stepsUntilEncounter -= stepValue;

        if (_stepsUntilEncounter <= 0)
        {
            TryTriggerEncounter();
            ResetStepCounter();
        }
    }

    public void ResetStepCounter()
    {
        _stepsUntilEncounter = Random.Range(
            config.minStepsBetweenEncounters,
            config.maxStepsBetweenEncounters + 1
        );
    }

    private void TryTriggerEncounter()
    {
        EncounterData encounter = GetRandomEncounter();
        if (encounter == null) return;
        TriggerEncounter(encounter);
    }

    private EncounterData GetRandomEncounter()
    {
        if (worldManager.CurrentZone == null) return null;
        List<EncounterData> encounters = worldManager.CurrentZone.encounters;
        if (encounters == null || encounters.Count == 0) return null;

        // Weighted random selection by encounterRate
        float totalWeight = 0f;
        foreach (var e in encounters)
            totalWeight += e.encounterRate;

        float roll = Random.Range(0f, totalWeight);
        float running = 0f;

        foreach (var e in encounters)
        {
            running += e.encounterRate;
            if (roll <= running)
                return e;
        }

        return encounters[^1];
    }

    private void TriggerEncounter(EncounterData encounter)
    {
        if (player.PlayerWorldState != PlayerWorldState.FreeRoam) return;

        player.StopMovement();
        player.SetState(PlayerWorldState.InBattle);
        WorldEvents.RaiseEncounterTriggered(encounter);
        LogManager.Trace($"[WorldManagerEncounter] Encounter triggered");
    }

    private void StartEncounterBattle(EncounterData encounter) 
    {
        BattleArgs.SetMini(
            homeTeamGuid : TeamLoadoutManager.Instance.ActiveLoadoutGuid, 
            awayTeamId : encounter.teamId,
            battleResultsType : BattleResultsType.Drop);
        sceneLoader.LoadGroup(sceneBattle);
    }

    #endregion

}
