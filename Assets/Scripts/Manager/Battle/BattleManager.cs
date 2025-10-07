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
    public BattleType CurrentType { get; private set; }
    public BattleType PreviousType { get; private set; }
    public int CurrentTeamSize { get; private set; }
    public event Action<BattlePhase, BattlePhase> OnPhaseChanged;

    public bool IsMovementFrozen { get; private set; } = false;
    public bool IsTimeFrozen { get; private set; } = false;

    [SerializeField] private List<Team> teams = new List<Team>();
    public List<Team> Teams => teams;


    [SerializeField] private float topOffset    = 1f;
    [SerializeField] private float bottomOffset = 1f;
    [SerializeField] private float leftOffset   = 1f;
    [SerializeField] private float rightOffset  = 1f;

    public event Action OnAllCharactersReady;
    private int charactersReadyMax;
    private int charactersReady;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        OnAllCharactersReady += HandleAllCharactersReady;

        BoundManager.TopOffset = topOffset;
        BoundManager.BottomOffset = bottomOffset;
        BoundManager.LeftOffset = leftOffset;
        BoundManager.RightOffset = rightOffset;
    }

    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        OnAllCharactersReady -= HandleAllCharactersReady;
    }

    public void StartBattle()
    {
        //Called on BattleCharacterSpawnPoint
        PreviousType = CurrentType;
        CurrentType = BattleArgs.BattleType;
        CurrentTeamSize = CurrentType == BattleType.Battle ? TeamManager.Instance.SizeBattle : TeamManager.Instance.SizeMiniBattle;
        charactersReadyMax = CurrentTeamSize*2;
        ResetBattle();
        teams.Add(TeamManager.Instance.GetTeam(BattleArgs.TeamId0));
        teams.Add(TeamManager.Instance.GetTeam(BattleArgs.TeamId1));

        for (int i = 0; i < teams.Count; i++) {
            Team team = teams[i];
            PopulateTeamWithCharacters(team, i, CurrentTeamSize);
        }
    }

    private void HandleAllCharactersReady()
    {
        //start kickoff etc
        ResetDefaultPositions();
    }

    private void PopulateTeamWithCharacters(Team team, int teamIndex, int teamSize)
    {
        foreach (var character in team.Characters)
        {
            BattleCharacterManager.Instance.ReturnCharacterToPool(character);
        }
        team.Characters.Clear();

        for (int i = 0; i < teamSize; i++)
        {
            int characterIndex = i;
            BattleCharacterManager.Instance.GetPooledCharacter((character) =>
            {
                if (character != null && characterIndex < team.CharacterDataList.Count)
                {
                    CharacterData characterData = team.CharacterDataList[characterIndex]; 
                    character.Initialize(characterData);
                    BattleCharacterManager.Instance.AssignCharacterToTeamBattle(character, team, characterIndex, teamIndex);
                    team.Characters.Add(character);
                
                    charactersReady++;
                    if (charactersReady >= charactersReadyMax)
                        OnAllCharactersReady?.Invoke(); 
                }
            });
        }
    }

    private void ResetBattle()
    {
        //BoundManager.Setup();
        teams.Clear();
        charactersReady = 0;
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
        int oppTeamIdx = 1 - character.TeamIndex;
        return teams[oppTeamIdx].Characters[0];
    }

    public TeamSide GetUserSide()
    {
        return TeamSide.Home; //Single Player only
    }

}
