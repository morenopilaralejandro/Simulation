using System;
using System.Collections;
using UnityEngine;
using Simulation.Enums.World;

/// <summary>
/// Central manager that orchestrates all overworld player systems.
/// Attach to a persistent GameObject (survives scene loads).
/// </summary>
public class WorldManagerPlayer : MonoBehaviour
{
    public static WorldManagerPlayer Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerWorldControllerComponent controllerComponent;
    [SerializeField] private WorldEncounterSystem encounterSystem;

    [Header("Configuration")]
    [SerializeField] private PlayerWorldConfig config;

    // ── State ──────────────────────────────────────────────
    public PlayerWorldState CurrentState { get; private set; } = PlayerWorldState.FreeRoam;
    public Vector3 PlayerPosition => playerTransform != null ? playerTransform.position : Vector3.zero;
    public FacingDirection PlayerFacing => controllerComponent != null
        ? controllerComponent.CurrentFacing
        : FacingDirection.Down;

    // ── Saved data ─────────────────────────────────────────
    private PlayerWorldSaveData _saveData = new();

    // ================================================================
    //  LIFECYCLE
    // ================================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ValidateReferences();
    }

    private void Start()
    {
        SetState(PlayerWorldState.FreeRoam);
    }

    private void Update()
    {
        if (CurrentState == PlayerWorldState.FreeRoam)
        {
            controllerComponent?.Tick(Time.deltaTime);
            encounterSystem?.Tick(controllerComponent.IsMoving, Time.deltaTime);
            //interactionHandler?.Tick();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // ================================================================
    //  STATE MANAGEMENT
    // ================================================================

    public void SetState(PlayerWorldState newState)
    {
        if (newState == CurrentState) return;

        PlayerWorldState previous = CurrentState;
        CurrentState = newState;

        // React to state transitions
        switch (newState)
        {
            case PlayerWorldState.FreeRoam:
                controllerComponent?.SetEnabled(true);
                break;

            case PlayerWorldState.InDialogue:
            case PlayerWorldState.InMenu:
            case PlayerWorldState.InCutscene:
            case PlayerWorldState.InBattle:
                controllerComponent?.SetEnabled(false);
                controllerComponent?.StopMovement();
                break;

            case PlayerWorldState.Transitioning:
                controllerComponent?.SetEnabled(false);
                controllerComponent?.StopMovement();
                break;

            case PlayerWorldState.Paused:
                controllerComponent?.SetEnabled(false);
                break;
        }

        WorldEvents.RaisePlayerStateChanged(previous, newState);
        LogManager.Trace($"[PlayerWorldManager] State: {previous} → {newState}");
    }

    public void TeleportPlayer(Vector3 destination)
    {
        if (playerTransform == null) return;

        // Disable character controller momentarily so position set works
        var cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerTransform.position = destination;

        if (cc != null) cc.enabled = true;

        WorldEvents.RaisePlayerTeleported(destination);
    }

    public void FaceDirection(FacingDirection direction)
    {
        controllerComponent?.SetFacing(direction);
    }

    // ================================================================
    //  ENCOUNTERS  (called by WorldEncounterSystem)
    // ================================================================

    public void TriggerEncounter(EncounterData encounter)
    {
        if (CurrentState != PlayerWorldState.FreeRoam) return;

        SetState(PlayerWorldState.InBattle);
        WorldEvents.RaiseEncounterTriggered(encounter);
        LogManager.Trace($"[PlayerWorldManager] Encounter triggered: {encounter.encounterName}");

        // Hand off to your battle system
        // BattleManager.Instance.StartBattle(encounter, OnBattleComplete);
    }

    public void OnBattleComplete(bool playerWon)
    {
        // Resume overworld
        SetState(PlayerWorldState.FreeRoam);
        encounterSystem?.ResetStepCounter();
    }

    // ================================================================
    //  SAVE / LOAD
    // ================================================================

    public PlayerWorldSaveData GetSaveData()
    {
        _saveData.lastPosition = PlayerPosition;
        _saveData.lastFacing = PlayerFacing;
        //_saveData.lastScene = SceneManager.GetActiveScene().name;
        return _saveData;
    }

    public void LoadFromSaveData(PlayerWorldSaveData data)
    {
        _saveData = data;
        //TransitionToZone(data.lastScene, data.lastPosition, data.lastFacing);
    }

    // ================================================================
    //  HELPERS
    // ================================================================

    private void CachePlayerComponents(GameObject go)
    {
        controllerComponent = go.GetComponent<PlayerWorldControllerComponent>();
    }

    private void ValidateReferences()
    {
        if (playerPrefab == null)
            LogManager.Error("[PlayerWorldManager] Player prefab is not assigned!");
        if (config == null)
            LogManager.Error("[PlayerWorldManager] Config is not assigned!");
    }
}
