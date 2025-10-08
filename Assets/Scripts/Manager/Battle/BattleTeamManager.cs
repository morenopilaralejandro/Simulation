using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Simulation.Enums.Character;

public class BattleTeamManager : MonoBehaviour
{
    public static BattleTeamManager Instance { get; private set; }

    private Dictionary<TeamSide, Team> teams = new Dictionary<TeamSide, Team>();
    public Dictionary<TeamSide, Team> Teams => teams;

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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AssignTeamToSide(Team team, TeamSide teamSide) {
        teams[teamSide] = team;
        TeamEvents.RaiseAssignTeamToSide(team, teamSide);
    }

    public TeamSide GetUserSide()
    {
        return TeamSide.Home; //Single Player only
    }

    public void Reset()
    {
        teams.Clear();
    }
}
