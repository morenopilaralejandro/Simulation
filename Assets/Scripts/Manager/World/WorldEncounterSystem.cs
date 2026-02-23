using UnityEngine;

public class WorldEncounterSystem : MonoBehaviour
{
    public void Tick(bool isMoving, float deltaTime) {}
    public void ResetStepCounter() {}

    /*
    //use current zone
    //have csv from excel to know the encounter info for each zone


    [SerializeField] private PlayerWorldConfig config;
    private int minStepsBetweenEncounters = 10;
    private int maxStepsBetweenEncounters = 30;

    private int _stepsUntilEncounter;
    private float _distanceAccumulator;
    private EncounterZone _currentZone;

    private const float STEP_DISTANCE = 1f; // 1 unit = 1 "step"

    private void Start()
    {
        ResetStepCounter();
    }

    /// <summary>Called each frame from PlayerWorldManager.</summary>
    public void Tick(bool isMoving, float deltaTime)
    {
        if (!isMoving || _currentZone == null) return;

        var movement = PlayerWorldManager.Instance
                        .GetComponent<PlayerMovementController>()
                        ?? FindObjectOfType<PlayerMovementController>();

        if (movement == null) return;

        float speed = movement.IsRunning ? 7f : 4f; // simplified
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

    public void SetEncounterZone(EncounterZone zone) => _currentZone = zone;
    public void ClearEncounterZone() => _currentZone = null;

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
        if (_currentZone == null) return;

        EncounterData encounter = _currentZone.GetRandomEncounter();
        if (encounter != null)
        {
            PlayerWorldManager.Instance.TriggerEncounter(encounter);
        }
    }

    public EncounterData GetRandomEncounter()
    {
        if (encounters == null || encounters.Length == 0) return null;

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
    
    */
}
