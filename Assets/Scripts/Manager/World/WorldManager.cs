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

    #region Fields

    [Header("Player System")]
    [SerializeField] private PlayerWorldConfig playerWorldConfig;

    [Header("Zone System")]
    [SerializeField] private OverworldDefinition overworldDefinition;

    [Header("Encounter System")]
    [SerializeField] private SceneGroup sceneBattle;

    private WorldManagerPlayer playerSystem;
    private WorldManagerZone zoneSystem;
    private WorldManagerZoneTracker zoneTrackerSystem;
    private WorldManagerEncounter encounterSystem;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy() 
    {
        encounterSystem.Unsubscribe();
    }


    private void Start()
    {
        playerSystem = new WorldManagerPlayer(playerWorldConfig);
        zoneTrackerSystem = new WorldManagerZoneTracker(overworldDefinition);
        zoneSystem = new WorldManagerZone(overworldDefinition);
        encounterSystem = new WorldManagerEncounter(sceneBattle);
        encounterSystem.Subscribe();

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        if (WorldArgs.WorldState == WorldState.InEncounter)
        {
            ReturnFromEncounter();
            return;
        }

        if (overworldDefinition.startingZone != null)
        {
            await LoadZone(overworldDefinition.startingZone, overworldDefinition.startingSpawnId);
            PlayerWorldEntity.SetControlEnabled(true);
        }
        else
        {
            LogManager.Error("[WorldManager] No starting zone assigned!");
        }
    }

    #endregion


    #region API

    // playerSystem
    public PlayerWorldConfig PlayerWorldConfig => playerSystem.PlayerWorldConfig;
    public PlayerWorldEntity PlayerWorldEntity => playerSystem.PlayerWorldEntity;

    // zoneSystem
    public WorldState CurrentState => zoneSystem.CurrentState;
    public void SetState(WorldState state) => zoneSystem.SetState(state);
    public void TransitionToZone(string zoneId, string spawnId) => zoneSystem.TransitionToZone(zoneId, spawnId);
    public Task LoadZoneAtPosition(ZoneDefinition zone, Vector3 position, Vector2 facing) =>
        zoneSystem.LoadZoneAtPosition(zone, position, facing);
    public Task LoadZone(ZoneDefinition zone, string spawnId) => zoneSystem.LoadZone(zone, spawnId);
    public Task<bool> UnloadCurrentZone() => zoneSystem.UnloadCurrentZone();
    public ZoneDefinition FindZone(string zoneId) => zoneSystem.FindZone(zoneId);
    public bool IsTransitioning => zoneSystem.IsTransitioning;
    public void SetIsTransitioning(bool boolValue) => zoneSystem.SetIsTransitioning(boolValue);
    public Task FadeIn() => zoneSystem.FadeInAsync();
    public Task FadeOut() => zoneSystem.FadeOutAsync();

    // zoneTrackerSystem
    public ZoneDefinition CurrentZone => zoneTrackerSystem.CurrentZone;
    public ZoneDefinition PreviousZone => zoneTrackerSystem.PreviousZone;
    public string ZoneName => zoneTrackerSystem.ZoneName;
    public void UpdatePlayerPosition(Vector3 worldPos) => zoneTrackerSystem.UpdatePlayerPosition(worldPos);
    public ZoneDefinition GetZoneAtPosition(Vector3 worldPos) => zoneTrackerSystem.GetZoneAtPosition(worldPos);
    public void SetZone(ZoneDefinition newZone) => zoneTrackerSystem.SetZone(newZone);

    // encounterSystem
    public void ReturnFromEncounter() => encounterSystem.ReturnFromEncounter();
    public void TickEncounter(bool isMoving, float speed, float deltaTime) => encounterSystem.Tick(isMoving, speed, deltaTime);
    public void OnTileArrived(bool isRunning) => encounterSystem.OnTileArrived(isRunning);
    public void ResetStepCounter() => encounterSystem.ResetStepCounter();

    #endregion

}
