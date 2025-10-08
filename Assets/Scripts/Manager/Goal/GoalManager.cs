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

    private Dictionary<TeamSide, Goal> goals = new Dictionary<TeamSide, Goal>();
    public Dictionary<TeamSide, Goal> Goals => goals;

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
    }

    public void SetGoal(Goal goal, TeamSide teamSide) 
    {
        goals[teamSide] = goal;
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

    public float GetDistanceToOppOpponentZ(Character character)
    {
        Transform goal = GetOpponentGoal(character).transform;
        return Mathf.Abs(character.transform.position.z - goal.position.z);
    }

    public Character GetAllyKeeper(Character character)
    {
        return BattleManager.Instance.Teams[character.TeamSide].CharacterList[0];
    }

    public Character GetOpponentKeeper(Character character)
    {
        return BattleManager.Instance.Teams[character.GetOpponentSide()].CharacterList[0];
    }

}
