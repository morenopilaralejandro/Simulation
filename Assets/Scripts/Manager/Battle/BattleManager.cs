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

    public Dictionary<TeamSide, Team> Teams => BattleTeamManager.Instance.Teams;

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

        BattleTeamManager.Instance.AssignTeamToSide(TeamManager.Instance.GetTeam(BattleArgs.TeamId0), TeamSide.Home);
        BattleTeamManager.Instance.AssignTeamToSide(TeamManager.Instance.GetTeam(BattleArgs.TeamId1), TeamSide.Away);
        BattleTeamManager.Instance.AssignVariants();

        foreach (Team team in Teams.Values) 
        {
            PopulateTeamWithCharacters(team, CurrentTeamSize);
        }
    }

    private void HandleAllCharactersReady()
    {
        //start kickoff etc
        ResetDefaultPositions();
    }

    private void PopulateTeamWithCharacters(Team team, int teamSize)
    {
        foreach (var character in team.CharacterList)
        {
            BattleCharacterManager.Instance.ReturnCharacterToPool(character);
        }
        team.CharacterList.Clear();

        for (int i = 0; i < teamSize; i++)
        {
            int characterIndex = i;
            BattleCharacterManager.Instance.GetPooledCharacter((character) =>
            {
                if (character != null && characterIndex < team.CharacterDataList.Count)
                {
                    CharacterData characterData = team.CharacterDataList[characterIndex]; 
                    character.Initialize(characterData);
                    BattleCharacterManager.Instance.AssignCharacterToTeamBattle(character, team, characterIndex);
                    team.CharacterList.Add(character);
                
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
        BattleTeamManager.Instance.Reset();
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
        foreach (Team team in Teams.Values) 
        {
            foreach (var character in team.CharacterList)
            {
                BattleCharacterManager.Instance.ResetCharacterPosition(character);
            }
        }
    }

    public void SetGamePhase(BattlePhase newPhase)
    {
        if (CurrentPhase != newPhase)
        {
            LogManager.Debug("[BattleManager] BattlePhase: " + newPhase);
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

}
