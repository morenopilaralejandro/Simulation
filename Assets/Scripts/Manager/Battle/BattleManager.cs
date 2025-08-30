using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public BattlePhase CurrentPhase { get; private set; }
    public BattlePhase PreviousPhase { get; private set; }
    public event Action<BattlePhase, BattlePhase> OnPhaseChanged;

    public bool IsMovementFrozen { get; private set; } = false;
    public bool IsTimeFrozen { get; private set; } = false;

    [SerializeField] private List<Team> teams = new List<Team>();
    public List<Team> Teams => teams;


    [SerializeField] private float topOffset    = 1f;
    [SerializeField] private float bottomOffset = 1f;
    [SerializeField] private float leftOffset   = 1f;
    [SerializeField] private float rightOffset  = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        BoundManager.TopOffset = topOffset;
        BoundManager.BottomOffset = bottomOffset;
        BoundManager.LeftOffset = leftOffset;
        BoundManager.RightOffset = rightOffset;

        BattleArgs.TeamId0 = "Faith";
        BattleArgs.TeamId0 = "Crimson";
    }

    void Start()
    {
        //BattleBallManager.Instance.Spawn();
        //StartBattle();
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBattle()
    {
        Reset();
        teams.Add(TeamManager.Instance.GetTeam(BattleArgs.TeamId0));
        teams.Add(TeamManager.Instance.GetTeam(BattleArgs.TeamId1));
        //StartKickoff(teams[0]);
    }

    private void SetCameraForMyTeam(int myTeamIndex)
    {

    }

    private void OfflineSpawn() 
    {
        SpawnCharacters_Singleplayer();
        BattleBallManager.Instance.Spawn();
        InitializeTeamCharacters();
    }

    public void AddCharacterToTeam(Character character, int teamIndex) 
    {
        teams[teamIndex].characters.Add(character);
    }

    private void SpawnCharacters_Singleplayer()
    {
        for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
        {
            Team team = teams[teamIndex];
            team.characters.Clear();

            ControlType controlType = (teamIndex == 0) ? ControlType.LocalHuman : ControlType.Ai;

            for (int j = 0; j < team.PlayerDataList.Count; j++)
            {
                BattleCharacterManager.Instance.SpawnCharacter_Singleplayer(teamIndex, controlType, team.Formation.FormationCoords[j]);
            }
        }
    }

    public void InitializeTeamCharacters()
    {
        for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
        {
            Team team = teams[teamIndex];
            for (int charIndex = 0; charIndex < team.characters.Count; charIndex++)
            {
                Character character = team.characters[charIndex];
                CharacterData characterData = team.CharacterDataList[charIndex];
                bool isKeeper = (charIndex == 0);
                
                //call event here OnAssignToTeamBattle
                BattleCharacterManager.Instance.InitializeCharacter(
                    character,
                    characterData,
                    teamIndex,
                    team,
                    isKeeper
                );
            }
        }
    }

    private void ResetBattle()
    {
        BoundManager.Setup();
        teams.Clear();
        //Reset timers
    }

    private void ResetDefaultPositions()
    {
        ResetPlayerPositions();
        BattleBallManager.Instance.ResetPosition();
    }

    private void ResetPlayerPositions()
    {
        foreach (var team in teams) 
        {
            foreach (var character in team.Characters)
            {
                BattleCharacterManager.Instance.ResetPosition(character);
            }
        }
    }

    public void SetGamePhase(BattlePhase newPhase)
    {
        if (CurrentPhase != newPhase)
        {
            LogManager.Debug("BattlePhase: " + newPhase);
            PreviousPhase = CurrentPhase;
            CurrentPhase = newPhase;
            OnPhaseChanged?.Invoke(CurrentPhase, PreviousPhase);
        }
    }

    public void Freeze()
    {
        IsMovementFrozen = true;
        IsTimeFrozen = true;
    }

    public void Unfreeze()
    {
        IsMovementFrozen = false;
        IsTimeFrozen = false;
    }


    public Player GetOppKeeper(Player player)
    {
        int oppTeamIdx = 1 - player.TeamIndex;
        return teams[oppTeamIdx].players[0];
    }

    public int GetLocalTeamIndex()
    {
        return 0; //Single Player only
    }
*/
}
