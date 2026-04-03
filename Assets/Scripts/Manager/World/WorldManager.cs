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

    [Header("Encounter System")]
    [SerializeField] private SceneGroup sceneBattle;

    private WorldManagerRealm realmSystem;
    private WorldManagerPlayer playerSystem;
    private WorldManagerZone zoneSystem;
    private WorldManagerZoneTracker zoneTrackerSystem;
    private WorldManagerEncounter encounterSystem;
    private WorldManagerPersistance persistanceSystem;

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
        realmSystem = new WorldManagerRealm();

        OverworldDefinition overworldDefinition = GetOverworldDefinition();

        playerSystem = new WorldManagerPlayer(playerWorldConfig);
        zoneTrackerSystem = new WorldManagerZoneTracker(overworldDefinition);
        zoneSystem = new WorldManagerZone(overworldDefinition);
        encounterSystem = new WorldManagerEncounter(sceneBattle);
        encounterSystem.Subscribe();
        persistanceSystem = new WorldManagerPersistance();

        InitializeAsync(overworldDefinition);
    }

    private async void InitializeAsync(OverworldDefinition overworldDefinition)
    {
        if (PersistenceManager.Instance.IsNewGame())
        {
            await LoadZone(overworldDefinition.startingZone, overworldDefinition.startingSpawnId);
            PlayerWorldEntity.SetControlEnabled(true);
        }
        else
        {
            LoadZoneFromUnloaded();
        }
    }

    #endregion


    #region API

    // realmSystem
    public Realm CurrentRealm => realmSystem.CurrentRealm;
    public void SetRealm(Realm realm) => realmSystem.SetRealm(realm);
    public OverworldDefinition GetOverworldDefinition() => realmSystem.GetOverworldDefinition();

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
    public void LoadZoneFromUnloaded() => zoneSystem.LoadZoneFromUnloaded();

    // zoneTrackerSystem
    public ZoneDefinition CurrentZone => zoneTrackerSystem.CurrentZone;
    public ZoneDefinition PreviousZone => zoneTrackerSystem.PreviousZone;
    public string ZoneName => zoneTrackerSystem.ZoneName;
    public void UpdatePlayerPosition(Vector3 worldPos) => zoneTrackerSystem.UpdatePlayerPosition(worldPos);
    public ZoneDefinition GetZoneAtPosition(Vector3 worldPos) => zoneTrackerSystem.GetZoneAtPosition(worldPos);
    public void SetZone(ZoneDefinition newZone) => zoneTrackerSystem.SetZone(newZone);

    // encounterSystem
    public void TickEncounter(bool isMoving, float speed, float deltaTime) => encounterSystem.Tick(isMoving, speed, deltaTime);
    public void OnTileArrived(bool isRunning) => encounterSystem.OnTileArrived(isRunning);
    public void ResetStepCounter() => encounterSystem.ResetStepCounter();

    // persistanceSystem
    public SaveDataWorldSystem Export() => persistanceSystem.Export();
    public void Import(SaveDataWorldSystem saveData) => persistanceSystem.Import(saveData);

    #endregion

}
