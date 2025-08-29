using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

/// <summary>
/// Handles calculations related to the top and bottom goals based on the team
/// </summary>

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance { get; private set; }

    private List<Goal> goals;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        goals = new List<Goal>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    public void Reset() 
    {
        goals.Clear();
    }

    public void AddGoal(Goal goal) 
    {
        // Make sure goals list has the right size
        if (goals.Count == 0)
        {
            goals.AddRange(new Goal[BattleManager.Instance.Teams.Count]);
        }

        // Index mapping: Bottom = Team 0, Top = Team 1
        int teamIndex = (goal.GoalPlacement == GoalPlacement.Bottom) ? 0 : 1;

        goals[teamIndex] = goal;
        goal.Team = BattleManager.Instance.Teams[teamIndex];
    }

    public Goal GetAllyGoal(Character character)
    {
        return goals[character.GetTeamIndex()];  
    }

    public Goal GetOpponentGoal(Character character)
    {
        return goals[1 - character.GetTeamIndex()];
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

}
