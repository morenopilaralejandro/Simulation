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

    private float keeperGoalDistanceX = 2f;
    private float keeperGoalDistanceZ = 2f;
    private float shootDistance = 5f;
    private Dictionary<TeamSide, Goal> goals = new ();
    private Dictionary<TeamSide, Character> keepers = new ();
    
    public Dictionary<TeamSide, Goal> Goals => goals;
    public Dictionary<TeamSide, Character> Keepers => keepers;

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

    public void Reset() 
    {
        goals.Clear();
        keepers.Clear();
    }

    public void SetGoal(Goal goal) 
    {
        goals[goal.TeamSide] = goal;
    }

    public void SetKeeper(Character character, TeamSide teamSide) 
    {
        keepers[teamSide] = character;
    }

    public Goal GetAllyGoal(Character character)
    {
        return goals[character.TeamSide];  
    }

    public Goal GetOpponentGoal(Character character)
    {
        return goals[character.GetOpponentSide()];
    }

    public float GetDistanceToAllyGoal(Character character)
    {
        Transform goal = GetAllyGoal(character).transform;
        return Vector3.Distance(character.transform.position, goal.position);
    }

    public float GetDistanceToAllyGoalX(Character character)
    {
        Transform goal = GetAllyGoal(character).transform;
        return Mathf.Abs(character.transform.position.x - goal.position.x);
    }

    public float GetDistanceToAllyGoalZ(Character character)
    {
        Transform goal = GetAllyGoal(character).transform;
        return Mathf.Abs(character.transform.position.z - goal.position.z);
    }

    public float GetDistanceToOpponentGoal(Character character)
    {
        Transform goal = GetOpponentGoal(character).transform;
        return Vector3.Distance(character.transform.position, goal.position);
    }

    public float GetDistanceToOppOpponentGoalX(Character character)
    {
        Transform goal = GetOpponentGoal(character).transform;
        return Mathf.Abs(character.transform.position.x - goal.position.x);
    }

    public float GetDistanceToOppOpponentGoalZ(Character character)
    {
        Transform goal = GetOpponentGoal(character).transform;
        return Mathf.Abs(character.transform.position.z - goal.position.z);
    }

    public Character GetAllyKeeper(Character character)
    {
        return keepers[character.TeamSide];
    }

    public Character GetOpponentKeeper(Character character)
    {
        return keepers[character.GetOpponentSide()];
    }

    public bool IsInOwnPenaltyArea(Character character)
    {
        float distanceX = GetDistanceToAllyGoalX(character);
        float distanceZ = GetDistanceToAllyGoalZ(character);
        return distanceX <= keeperGoalDistanceX && distanceZ <= keeperGoalDistanceZ;
    }

    public bool IsInShootDistance(Character character)
    {
        float distance = GetDistanceToOppOpponentGoalZ(character);
        return distance <= shootDistance;
    }

}
