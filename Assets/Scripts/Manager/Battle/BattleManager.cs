using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] private BattlePhase currentPhase;
    [SerializeField] private BattlePhase lastPhase;
    [SerializeField] private BattleType currentType;
    [SerializeField] private BattleType lastType;
    [SerializeField] private int currentTeamSize;
    [SerializeField] private bool isMovementFrozen;
    [SerializeField] private bool isTimeFrozen;
    [SerializeField] private float timeDefault = 0f;
    [SerializeField] private float timeCurrent = 1800f;
    [SerializeField] private float timeLimit = 1800f;
    [SerializeField] private Dictionary<TeamSide, int> scoreDict;

    public BattlePhase CurrentPhase => currentPhase;
    public BattlePhase LastPhase => lastPhase;
    public BattleType CurrentType => currentType;
    public BattleType LastType => lastType;
    public int CurrentTeamSize => currentTeamSize;
    public bool IsMovementFrozen => isMovementFrozen;
    public bool IsTimeFrozen => isTimeFrozen;

    public Dictionary<TeamSide, Team> Teams => BattleTeamManager.Instance.Teams;
    public Dictionary<TeamSide, Character> TargetedCharacter => CharacterTargetManager.Instance.TargetedCharacter;
    public Dictionary<TeamSide, Character> ControlledCharacter => CharacterChangeControlManager.Instance.ControlledCharacter;
    public Ball Ball => BattleBallManager.Instance.Ball;
    public TeamSide GetUserSide() => BattleTeamManager.Instance.GetUserSide();

    public event Action<BattlePhase, BattlePhase> OnBattlePhaseChanged;
    public event Action OnAllCharactersReady;
    private int charactersReadyMax;
    private int charactersReady;

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetScore();
        OnAllCharactersReady += HandleAllCharactersReady;
    }

    void Start()
    {
        SetTeamSize();
    }
    
    void Update()
    {
        if (!isTimeFrozen)
        {
            if (timeCurrent <= timeLimit)
            {
                timeCurrent++;
                BattleUIManager.Instance.UpdateTimerDisplay(timeCurrent);
            }
            CheckEndGame();
        }
    }

    private void OnDestroy()
    {
        OnAllCharactersReady -= HandleAllCharactersReady;
    }
    #endregion

    #region Generic
    public void Freeze()
    {
        isMovementFrozen = true;
        isTimeFrozen = true;
    }

    public void Unfreeze()
    {
        isMovementFrozen = false;
        isTimeFrozen = false;
    }

    public void SetBattlePhase(BattlePhase newPhase)
    {
        if (currentPhase == newPhase) return;

        LogManager.Info($"[BattleManager] " + 
            $"BattlePhase changed to {newPhase}" , this);
        lastPhase = currentPhase;
        currentPhase = newPhase;
        OnBattlePhaseChanged?.Invoke(currentPhase, lastPhase);
    }

    private void SetBattleType(BattleType newType)
    {
        if (currentType == newType) return;

        LogManager.Info($"[BattleManager] " + 
            $"BattleType changed to {newType}" , this);
        lastType = currentType;
        currentType = newType;
        SetTeamSize();
    }

    private void SetTeamSize()
    {
        currentTeamSize = currentType == BattleType.Battle ? TeamManager.Instance.SizeBattle : TeamManager.Instance.SizeMiniBattle;
        charactersReadyMax = currentTeamSize*2;
    }
    #endregion

    #region StartBattle
    public void StartBattle()
    {
        //Called on BattleCharacterSpawnPoint
        Freeze();
        SetBattleType(BattleArgs.BattleType);
        //SetBattlePhase(BattlePhase.Battle);
        ResetBattle();

        BattleTeamManager.Instance.AssignTeamToSide(
            TeamManager.Instance.GetTeam(BattleArgs.TeamId0), 
            TeamSide.Home);
        BattleTeamManager.Instance.AssignTeamToSide(
            TeamManager.Instance.GetTeam(BattleArgs.TeamId1), 
            TeamSide.Away);
        BattleTeamManager.Instance.AssignVariants();

        foreach (Team team in Teams.Values)
        {
            BattleUIManager.Instance.SetTeam(team);
            PopulateTeamWithCharacters(team, CurrentTeamSize);
        }
    }

    private void ResetBattle()
    {
        BattleTeamManager.Instance.Reset();
        charactersReady = 0;
        ResetScore();
        ResetTimer();
    }

    private void ResetScore() 
    {
        scoreDict = new Dictionary<TeamSide, int>
        {
            { TeamSide.Home, 0 },
            { TeamSide.Away, 0 }
        };
        BattleUIManager.Instance.ResetScoreboard();
    }

    private void ResetTimer() 
    {
        timeCurrent = timeDefault;
        BattleUIManager.Instance.UpdateTimerDisplay(timeCurrent);
    }

    public void GoalScored(Goal goal)
    {
        Team scorringTeam = null;
        if (goal.TeamSide == TeamSide.Home) 
        {
            scorringTeam = Teams[TeamSide.Away];
        }
        else {
            scorringTeam = Teams[TeamSide.Home];
        }
    
        scoreDict[scorringTeam.TeamSide]++;
        BattleUIManager.Instance.UpdateScoreDisplay(
            scorringTeam, 
            scoreDict[scorringTeam.TeamSide]);

        StartCoroutine(GoalSequence(goal.TeamSide));
    }

    private void CheckEndGame() 
    {
        if (timeCurrent >= timeLimit)
        {
            StartCoroutine(TimeOverSequence());
        }
    }
    #endregion

    #region Sequence 
    private IEnumerator GoalSequence(TeamSide kickoffTeamSide)
    {
        float duration = 2f;
        isTimeFrozen = true;
        //AudioManager.Instance.PlayBgm("BgmOle");
        //panelGoalMessage.SetActive(true);
        //textGoalMessage.Play("TextGoalSlide", -1, 0f);

        yield return new WaitForSeconds(duration);
        //panelGoalMessage.SetActive(false);
        isTimeFrozen = false;
        //StartKickoff(kickoffTeam);
    }

    private IEnumerator TimeOverSequence()
    {
        float duration = 2f;
        //isTimeFrozen = true;
        //AudioManager.Instance.PlayBgm("BgmTimeUp");
        ResetTimer();
        //panelTimeMessage.SetActive(true);
        //DuelLogManager.Instance.AddMatchEnd();

        yield return new WaitForSeconds(duration);
        //panelTimeMessage.SetActive(false);
        //SceneManager.LoadScene("GameOver");
    }
    #endregion

    #region Team and Ball
    private void HandleAllCharactersReady()
    {
        //start kickoff etc
        ResetDefaultPositions();
        CharacterChangeControlManager.Instance.SetControlledCharacter(
            Teams[TeamSide.Home].CharacterList[10], 
            TeamSide.Home);

        SetBattlePhase(BattlePhase.Battle);
        Unfreeze();
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
                    character.gameObject.name = character.CharacterId;
                    team.CharacterList.Add(character);
                
                    charactersReady++;
                    if (charactersReady >= charactersReadyMax)
                        OnAllCharactersReady?.Invoke(); 
                }
            });
        }
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
    #endregion

}
