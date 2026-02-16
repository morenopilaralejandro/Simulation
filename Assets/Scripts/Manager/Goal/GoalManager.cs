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
    private Dictionary<TeamSide, Goal> goals = new ();
    private Dictionary<TeamSide, CharacterEntityBattle> keepers = new ();
    
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

    public Goal GetOwnGoal(CharacterEntityBattle character)
    {
        return goals[character.TeamSide];  
    }

    public Goal GetOpponentGoal(CharacterEntityBattle character)
    {
        return goals[character.GetOpponentSide()];
    }

    public float GetDistanceToOwnGoal(CharacterEntityBattle character)
    {
        Transform goal = GetOwnGoal(character).transform;
        return Vector3.Distance(character.transform.position, goal.position);
    }

    public float GetDistanceToOwnGoalX(CharacterEntityBattle character)
    {
        Transform goal = GetOwnGoal(character).transform;
        return Mathf.Abs(character.transform.position.x - goal.position.x);
    }

    public float GetDistanceToOwnGoalZ(CharacterEntityBattle character)
    {
        Transform goal = GetOwnGoal(character).transform;
        return Mathf.Abs(character.transform.position.z - goal.position.z);
    }

    public float GetDistanceToOpponentGoal(CharacterEntityBattle character)
    {
        Transform goal = GetOpponentGoal(character).transform;
        return Vector3.Distance(character.transform.position, goal.position);
    }

    public float GetDistanceToOpponentGoalX(CharacterEntityBattle character)
    {
        Transform goal = GetOpponentGoal(character).transform;
        return Mathf.Abs(character.transform.position.x - goal.position.x);
    }

    public float GetDistanceToOpponentGoalZ(CharacterEntityBattle character)
    {
        Transform goal = GetOpponentGoal(character).transform;
        return Mathf.Abs(character.transform.position.z - goal.position.z);
    }

    public CharacterEntityBattle GetOwnKeeper(CharacterEntityBattle character)
    {
        return keepers[character.TeamSide];
    }

    public CharacterEntityBattle GetOpponentKeeper(CharacterEntityBattle character)
    {
        return keepers[character.GetOpponentSide()];
    }

    public bool IsInOwnPenaltyArea(CharacterEntityBattle character)
    {
        float distanceX = GetDistanceToOwnGoalX(character);
        float distanceZ = GetDistanceToOwnGoalZ(character);
        return distanceX <= keeperGoalDistanceX && distanceZ <= keeperGoalDistanceZ;
    }

    public bool IsInShootDistance(CharacterEntityBattle character)
    {
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
