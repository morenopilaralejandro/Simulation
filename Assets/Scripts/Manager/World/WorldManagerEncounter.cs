using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Battle;
using Simulation.Enums.World;

public class WorldManagerEncounter : MonoBehaviour
{
    //TODO have csv from excel to know the encounter info for each zone
    public static WorldManagerEncounter Instance { get; private set; }

    [SerializeField] private SceneGroup sceneBattle;

    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;
    private ZoneTracker zoneTracker;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        BattleEvents.OnBattleEnd += HandleBattleEnd;
    }

    private void Start() 
    {
        config = WorldManagerPlayer.Instance.PlayerWorldConfig;
        playerWorldEntity = WorldManagerPlayer.Instance.PlayerWorldEntity;
        zoneTracker = ZoneTracker.Instance;

        ResetStepCounter();
    }

    private void OnDestroy() 
    {
        BattleEvents.OnBattleEnd -= HandleBattleEnd;
    }

    #region Events
    private void HandleBattleEnd() 
    {
        //Resume overworld
        //playerWorldEntity.SetState(PlayerWorldState.FreeRoam);
        //WorldManager.ReturnFromEncounter()
    }
    #endregion

    private int _stepsUntilEncounter;
    private float _distanceAccumulator;
    private const float STEP_DISTANCE = 1f;

    /// <summary>Called from PlayerWorldComponentController.</summary>
    public void Tick(bool isMoving, float speed, float deltaTime)
    {
        if (!isMoving || zoneTracker.CurrentZone == null) return;

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

    public void ResetStepCounter()
    {
        _stepsUntilEncounter = Random.Range(
            config.minStepsBetweenEncounters,
            config.maxStepsBetweenEncounters + 1
        );
        _distanceAccumulator = 0f;
    }

    private void TryTriggerEncounter()
    {
        EncounterData encounter = GetRandomEncounter();
        if (encounter == null) return;
        TriggerEncounter(encounter);
    }

    public EncounterData GetRandomEncounter()
    {
        if (zoneTracker.CurrentZone == null) return null;
        List<EncounterData> encounters = zoneTracker.CurrentZone.encounters;
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

    //event handled in WorldManager
    public void TriggerEncounter(EncounterData encounter)
    {
        if (playerWorldEntity.PlayerWorldState != PlayerWorldState.FreeRoam) return;

        playerWorldEntity.StopMovement();
        playerWorldEntity.SetState(PlayerWorldState.InBattle);
        WorldEvents.RaiseEncounterTriggered(encounter);
        LogManager.Trace($"[WorldManagerEncounter] Encounter triggered");
    }

    public void StartEncounterBattle(EncounterData encounter) 
    {
        BattleArgs.SetMini(
            "faith_selection", 
            encounter.teamId,
            battleResultsType : BattleResultsType.Drop);
        SceneLoader.Instance.LoadGroup(sceneBattle);
    }

}
