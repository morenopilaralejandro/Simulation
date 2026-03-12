using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

/// <summary>
/// Handles calculations related to the top and bottom goals based on the team
/// </summary>

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance { get; private set; }

    private float keeperGoalDistanceX = 4.05f;
    private float keeperGoalDistanceZ = 3.05f;
    private float shootDistance = 4f;
    private float shootDistanceFull = 4f;
    private float shootDistanceMini = 2.5f;
    private Dictionary<TeamSide, Goal> goals = new();
    private Dictionary<TeamSide, CharacterEntityBattle> keepers = new();

    public Dictionary<TeamSide, Goal> Goals => goals;
    public Dictionary<TeamSide, CharacterEntityBattle> Keepers => keepers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleStart -= HandleBattleStart;
    }

    private void HandleBattleStart()
    {
        BattleType currentMode = BattleManager.Instance.CurrentType;

        if (currentMode == BattleType.Mini)
        {
            shootDistance = shootDistanceMini;
        }
        else
        {
            shootDistance = shootDistanceFull;
        }
    }

    public void Reset()
    {
        goals.Clear();
        keepers.Clear();
    }

    public void SetGoal(Goal goal)
    {
        goals[goal.TeamSide] = goal;
    }

    public void SetKeeper(CharacterEntityBattle character, TeamSide teamSide)
    {
        keepers[teamSide] = character;
    }

    public bool HasGoal(TeamSide side)
    {
        return goals.ContainsKey(side);
    }

    public bool HasKeeper(TeamSide side)
    {
        return keepers.ContainsKey(side);
    }

    public Goal GetOwnGoal(CharacterEntityBattle character)
    {
        if (goals.TryGetValue(character.TeamSide, out Goal goal))
            return goal;

        LogManager.Warning($"[GoalManager] No goal found for team side: {character.TeamSide}");
        return null;
    }

    public Goal GetOpponentGoal(CharacterEntityBattle character)
    {
        TeamSide opponentSide = character.GetOpponentSide();

        if (goals.TryGetValue(opponentSide, out Goal goal))
            return goal;

        LogManager.Warning($"[GoalManager] No goal found for opponent side: {opponentSide}");
        return null;
    }

    public float GetDistanceToOwnGoal(CharacterEntityBattle character)
    {
        Goal goal = GetOwnGoal(character);
        if (goal == null) return float.MaxValue;

        return Vector3.Distance(character.transform.position, goal.transform.position);
    }

    public float GetDistanceToOwnGoalX(CharacterEntityBattle character)
    {
        Goal goal = GetOwnGoal(character);
        if (goal == null) return float.MaxValue;

        return Mathf.Abs(character.transform.position.x - goal.transform.position.x);
    }

    public float GetDistanceToOwnGoalZ(CharacterEntityBattle character)
    {
        Goal goal = GetOwnGoal(character);
        if (goal == null) return float.MaxValue;

        return Mathf.Abs(character.transform.position.z - goal.transform.position.z);
    }

    public float GetDistanceToOpponentGoal(CharacterEntityBattle character)
    {
        Goal goal = GetOpponentGoal(character);
        if (goal == null) return float.MaxValue;

        return Vector3.Distance(character.transform.position, goal.transform.position);
    }

    public float GetDistanceToOpponentGoalX(CharacterEntityBattle character)
    {
        Goal goal = GetOpponentGoal(character);
        if (goal == null) return float.MaxValue;

        return Mathf.Abs(character.transform.position.x - goal.transform.position.x);
    }

    public float GetDistanceToOpponentGoalZ(CharacterEntityBattle character)
    {
        Goal goal = GetOpponentGoal(character);
        if (goal == null) return float.MaxValue;

        return Mathf.Abs(character.transform.position.z - goal.transform.position.z);
    }

    public CharacterEntityBattle GetOwnKeeper(CharacterEntityBattle character)
    {
        if (keepers.TryGetValue(character.TeamSide, out CharacterEntityBattle keeper))
            return keeper;

        LogManager.Warning($"[GoalManager] No keeper found for team side: {character.TeamSide}");
        return null;
    }

    public CharacterEntityBattle GetOpponentKeeper(CharacterEntityBattle character)
    {
        TeamSide opponentSide = character.GetOpponentSide();

        if (keepers.TryGetValue(opponentSide, out CharacterEntityBattle keeper))
            return keeper;

        LogManager.Warning($"[GoalManager] No keeper found for opponent side: {opponentSide}");
        return null;
    }

    public bool IsInOwnPenaltyArea(CharacterEntityBattle character)
    {
        Goal goal = GetOwnGoal(character);
        if (goal == null) return false;

        float distanceX = GetDistanceToOwnGoalX(character);
        float distanceZ = GetDistanceToOwnGoalZ(character);
        return distanceX <= keeperGoalDistanceX && distanceZ <= keeperGoalDistanceZ;
    }

    public bool IsInShootDistance(CharacterEntityBattle character)
    {
        Goal goal = GetOpponentGoal(character);
        if (goal == null) return false;

        float distance = GetDistanceToOpponentGoalZ(character);
        return distance <= shootDistance;
    }

    #region Gizmos

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (goals == null || goals.Count == 0)
            return;

        foreach (var goalEntry in goals)
        {
            Goal goal = goalEntry.Value;
            if (goal == null)
                continue;

            Transform goalTransform = goal.transform;

            // -----------------------------
            // Penalty Area (Keeper Area)
            // -----------------------------
            Gizmos.color = Color.yellow;

            Vector3 penaltyCenter = new Vector3(
                goalTransform.position.x,
                goalTransform.position.y,
                goalTransform.position.z
            );

            Vector3 penaltySize = new Vector3(
                keeperGoalDistanceX * 2f,
                0.1f,
                keeperGoalDistanceZ * 2f
            );

            Gizmos.DrawWireCube(penaltyCenter, penaltySize);

            // -----------------------------
            // Shoot Distance Area
            // -----------------------------
            Gizmos.color = Color.red;

            // Direction based on team side
            float direction = goal.TeamSide == TeamSide.Away ? -1f : 1f;

            Vector3 shootCenter = goalTransform.position +
                                  new Vector3(0f, 0f, direction * shootDistance * 0.5f);

            Vector3 shootSize = new Vector3(
                penaltySize.x * 2f,
                0.1f,
                shootDistance
            );

            Gizmos.DrawWireCube(shootCenter, shootSize);
        }
    }
#endif

    #endregion
}
