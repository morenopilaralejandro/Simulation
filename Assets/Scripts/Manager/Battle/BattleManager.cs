using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

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
        BattleArgs.TeamId1 = "Crimson";
    }

    void Start()
    {
        //BattleBallManager.Instance.Spawn();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() 
    {
        if (BattleCharacterManager.Instance != null) {
            BattleCharacterManager.Instance.OnAllCharactersSpawned += HandleAllCharactersSpawned;
            LogManager.Trace("[BattleManager] Subscribed to OnAllCharactersSpawned.");
        }
    }

    void OnDisable() 
    {
        if (BattleCharacterManager.Instance != null)
            BattleCharacterManager.Instance.OnAllCharactersSpawned -= HandleAllCharactersSpawned;
    }

    public void StartBattle()
    {
        ResetBattle();
        teams.Add(TeamManager.Instance.GetTeam(BattleArgs.TeamId0));
        teams.Add(TeamManager.Instance.GetTeam(BattleArgs.TeamId1));
        OfflineSpawn();
        //StartKickoff(teams[0]);
    }

    private void SetCameraForMyTeam(int myTeamIndex)
    {

    }

    private void OfflineSpawn() 
    {
        SpawnCharacters_Singleplayer();
    }

    public void AddCharacterToTeam(Character character, int teamIndex) 
    {
        teams[teamIndex].Characters.Add(character);
    }

    private void SpawnCharacters_Singleplayer()
    {
        for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
        {
            Team team = teams[teamIndex];
            team.Characters.Clear();

            ControlType controlType = (teamIndex == 0) ? ControlType.LocalHuman : ControlType.AI;

            for (int j = 0; j < team.CharacterDataList.Count; j++)
            {
                BattleCharacterManager.Instance.SpawnCharacter_Singleplayer(teamIndex);
            }
        }
    }

    private void HandleAllCharactersSpawned()
    {
        LogManager.Trace("[BattleManager] HandleAllCharactersSpawned");
        InitializeTeamCharacters();
        ResetDefaultPositions();
        BattleBallManager.Instance.Spawn();
    }

    public void InitializeTeamCharacters()
    {
        for (int i = 0; i < teams.Count; i++)
        {
            int teamIndex = i;
            Team team = teams[teamIndex];            
            for (int j = 0; j < TeamManager.Instance.SizeBattle; j++)
            {
                int characterIndex = j;
                Character character = team.Characters[characterIndex];
                CharacterData characterData = team.CharacterDataList[characterIndex];
                character.Initialize(characterData);
                //reverse if necesary
                FormationCoord formationCoord = team.Formation.FormationCoords[characterIndex];
                ControlType controlType = (teamIndex == 0) ? ControlType.LocalHuman : ControlType.AI;
                bool isKeeper = (characterIndex == 0);
                
                TeamEvents.RaiseAssignToTeamBattle(character, team, teamIndex, formationCoord, controlType, isKeeper);
            }
        }
    }

    private void ResetBattle()
    {
        //BoundManager.Setup();
        teams.Clear();
        //Reset timers
    }

    private void ResetDefaultPositions()
    {
        ResetPlayerPositions();
        BattleBallManager.Instance.ResetBallPosition();
    }

    private void ResetPlayerPositions()
    {
        foreach (var team in teams) 
        {
            foreach (var character in team.Characters)
            {
                BattleCharacterManager.Instance.ResetCharacterPosition(character);
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


    public Character GetOppKeeper(Character character)
    {
        int oppTeamIdx = 1 - character.GetTeamIndex();
        return teams[oppTeamIdx].Characters[0];
    }

    public int GetLocalTeamIndex()
    {
        return 0; //Single Player only
    }

}
