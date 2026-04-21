using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.DeadBall;

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
    [SerializeField] private float timeCurrent = 0f;
    [SerializeField] private float timeLimit = 1800f;
    [SerializeField] private float timeScale = 30f;
    [SerializeField] private TimerHalf timerHalf;
    [SerializeField] private Dictionary<TeamSide, int> scoreDict;

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneBattle;
    [SerializeField] private SceneGroup sceneMainMenu;
    [SerializeField] private SceneGroup sceneGameOver;
    [SerializeField] private SceneGroup sceneBattleResults;
    private SceneLoader sceneLoader;

    public BattlePhase CurrentPhase => currentPhase;
    public BattlePhase LastPhase => lastPhase;
    public BattleType CurrentType => currentType;
    public BattleType LastType => lastType;
    public int CurrentTeamSize => currentTeamSize;
    public bool IsMovementFrozen => isMovementFrozen;
    public bool IsTimeFrozen => isTimeFrozen;

    public Dictionary<TeamSide, Team> Teams => battleTeamManager.Teams;
    public Dictionary<TeamSide, CharacterEntityBattle> TargetedCharacter => CharacterTargetManager.Instance.TargetedCharacter;
    public Dictionary<TeamSide, CharacterEntityBattle> ControlledCharacter => CharacterChangeControlManager.Instance.ControlledCharacter;
    public Ball Ball => BattleBallManager.Instance.Ball;
    public TeamSide GetUserSide() => battleTeamManager.GetUserSide();

    // Win condition
    private WinCondition winCondition;
    public WinCondition WinCondition => winCondition;

    private int charactersReadyMax;
    private int charactersReady;

    private BattleTeamManager battleTeamManager;

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
        BattleEvents.OnAllCharactersReady += HandleAllCharactersReady;
    }

    void Start()
    {
        sceneLoader = SceneLoader.Instance;
        battleTeamManager = BattleTeamManager.Instance;
        Freeze();
        SetTeamSize();
    }
    
    void Update()
    {
        if (isTimeFrozen) return;
            
        timeCurrent += Time.deltaTime * timeScale;
        BattleUIManager.Instance.UpdateTimerDisplay(timeCurrent);
        
        if(!Ball.IsTraveling)
            CheckEndGame();
    }

    private void OnDestroy()
    {
        BattleEvents.OnAllCharactersReady -= HandleAllCharactersReady;
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

        BattleEvents.RaiseBattlePhaseChanged(newPhase, currentPhase);

        LogManager.Info($"[BattleManager] " + 
            $"BattlePhase changed to {newPhase}" , this);
        lastPhase = currentPhase;
        currentPhase = newPhase;
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
        currentTeamSize = currentType == BattleType.Full ? TeamManager.SIZE_FULL : TeamManager.SIZE_MINI;
        charactersReadyMax = currentTeamSize * 2;
    }
    #endregion

    #region StartBattle
    public void StartBattle()
    {
        Freeze();
        SetBattleType(BattleArgs.BattleType);
        
        // Initialize win condition from BattleArgs
        winCondition = WinCondition.Create(
            BattleArgs.WinConditionType, 
            BattleArgs.WinConditionParams);
        LogManager.Info($"[BattleManager] WinCondition: {winCondition.Type}", this);

        ResetBattle();

        BattleFieldManager.Instance.InitializeField();

        battleTeamManager.AssignTeamToSide(
            battleTeamManager.ResolveTeamForSide(TeamSide.Home), 
            TeamSide.Home);
        battleTeamManager.AssignTeamToSide(
            battleTeamManager.ResolveTeamForSide(TeamSide.Away), 
            TeamSide.Away);
        battleTeamManager.AssignVariants();

        foreach (Team team in Teams.Values)
        {
            BattleUIManager.Instance.SetTeam(team);
            PopulateTeamWithCharacters(team, CurrentTeamSize);
        }
    }

    private void ResetBattle()
    {
        battleTeamManager.Reset();
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
        timerHalf = TimerHalf.First;
        BattleUIManager.Instance.UpdateTimerDisplay(timeCurrent);
        BattleUIManager.Instance.UpdateTimerHalfDisplay(timerHalf);

        // Hide half indicator if the win condition doesn't use two halves
        if (winCondition != null && !winCondition.HasTwoHalves)
            BattleUIManager.Instance.HideTimerHalf();
        else if (currentType == BattleType.Mini)
            BattleUIManager.Instance.HideTimerHalf();
    }

    private void StartSecondHalf() 
    {
        ResetTimer();
        timerHalf = TimerHalf.Second;
        BattleUIManager.Instance.UpdateTimerHalfDisplay(timerHalf);
        DuelLogManager.Instance.AddMatchHalf();
        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.Kickoff, TeamSide.Away);
    }

    public void GoalScored(Goal goal)
    {
        Team scoringTeam = null;
        if (goal.TeamSide == TeamSide.Home) 
        {
            scoringTeam = Teams[TeamSide.Away];
        }
        else 
        {
            scoringTeam = Teams[TeamSide.Home];
        }
    
        scoreDict[scoringTeam.TeamSide]++;
        BattleUIManager.Instance.UpdateScoreDisplay(
            scoringTeam, 
            scoreDict[scoringTeam.TeamSide]);

        BattleEvents.RaiseGoalScored(PossessionManager.Instance.LastCharacter);

        // Check if the win condition triggers an immediate end on goal
        if (winCondition.ShouldEndOnGoal(scoreDict, timerHalf))
        {
            Freeze();
            StartCoroutine(GoalWinSequence(scoringTeam.TeamSide));
        }
        else
        {
            StartCoroutine(GoalSequence(goal.TeamSide));
        }
    }

    private void CheckEndGame() 
    {
        if (!isTimeFrozen && timeCurrent >= timeLimit) 
        {
            Freeze();
            StartCoroutine(TimeOverSequence());
        }
    }

    private void EndGame()
    {   
        WinConditionResult result = winCondition.EvaluateResult(scoreDict, timerHalf);
        
        int homeScore = scoreDict[TeamSide.Home];
        int awayScore = scoreDict[TeamSide.Away];
        TeamSide userSide = GetUserSide();

        LogManager.Info($"[BattleManager] Game Ended — Home: {homeScore} Away: {awayScore}, " +
            $"User side: {userSide}, Result: {result}, WinCondition: {winCondition.Type}", this);

        SetBattlePhase(BattlePhase.End);
        BattleEvents.RaiseBattleEnd();

        var nextScene = sceneGameOver;
        switch (result)
        {
            case WinConditionResult.HomeWin:
                if (userSide == TeamSide.Home)
                    nextScene = sceneBattleResults;
                else
                    nextScene = sceneGameOver;
                break;
            case WinConditionResult.AwayWin:
                if (userSide == TeamSide.Away)
                    nextScene = sceneBattleResults;
                else
                    nextScene = sceneGameOver;
                break;
            case WinConditionResult.Draw:
            default:
                // Handle draw — you may want a different scene or fallback
                nextScene = sceneGameOver;
                break;
        }

        sceneLoader.LoadGroup(nextScene);
    }

    private void ForceEndGame(TeamSide winnerSide)
    {
        if (currentPhase == BattlePhase.End) return;        

        SetBattlePhase(BattlePhase.End);
        BattleEvents.RaiseBattleEnd();

        var nextScene = sceneGameOver;

        TeamSide userSide = GetUserSide();
        if (winnerSide == userSide)
            nextScene = sceneBattleResults;
        else
            nextScene = sceneGameOver;

        sceneLoader.LoadGroup(nextScene);
    }

    public void ForfeitBattle()
    {
        LogManager.Info("[BattleManager] User forfeits the battle.", this);
        
        Freeze();
        
        TeamSide userSide = GetUserSide();
        TeamSide opponentSide = (userSide == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;

        StartCoroutine(ForfeitSequence(opponentSide));
    }

    #endregion

    #region Sequence 
    private IEnumerator GoalSequence(TeamSide kickoffTeamSide)
    {
        float duration = 
            SettingsManager.Instance.IsAutoBattleEnabled ? 0.8f : 2f;
        isTimeFrozen = true;
        AudioManager.Instance.PlayBgm("bgm-ole");
        BattleUIManager.Instance.SetMessageActive(MessageType.Goal, true);

        yield return new WaitForSeconds(duration);

        BattleUIManager.Instance.SetMessageActive(MessageType.Goal, false);
        isTimeFrozen = false;

        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.Kickoff, kickoffTeamSide);
    }

    /// <summary>
    /// Goal was scored and the win condition says the game should end immediately.
    /// Show goal celebration, then end the game.
    /// </summary>
    private IEnumerator GoalWinSequence(TeamSide winningSide)
    {
        float duration = 
            SettingsManager.Instance.IsAutoBattleEnabled ? 0.8f : 2f;
        
        AudioManager.Instance.PlayBgm("bgm-ole");
        BattleUIManager.Instance.SetMessageActive(MessageType.Goal, true);

        yield return new WaitForSeconds(duration);

        BattleUIManager.Instance.SetMessageActive(MessageType.Goal, false);

        EndGame();
    }

    private IEnumerator TimeOverSequence()
    {
        float duration = 1.5f;

        // Cancel lingering duel
        if (!DuelManager.Instance.IsResolved)
            Ball.CancelTravel();

        // Determine the message to show
        MessageType messageType = MessageType.TimeUp;

        // Only show HalfTime/FullTime if the win condition uses two halves
        if (winCondition.HasTwoHalves)
        {
            messageType = timerHalf == TimerHalf.First 
                ? MessageType.HalfTime 
                : MessageType.FullTime;
        }

        BattleUIManager.Instance.SetMessageActive(messageType, true);

        // Play sound effect
        if (messageType == MessageType.HalfTime)
        {
            AudioManager.Instance.PlaySfx("sfx-whistle_single");
        }
        else
        {
            duration = 2f;
            AudioManager.Instance.PlaySfx("sfx-whistle_triple");
        }

        yield return new WaitForSeconds(duration);

        BattleUIManager.Instance.SetMessageActive(messageType, false);

        // Ask the win condition whether the game should end
        if (winCondition.ShouldEndOnTimeOver(scoreDict, timerHalf))
        {
            EndGame();
        }
        else
        {
            // Win condition says continue — start the second half
            StartSecondHalf();
        }
    }

    private IEnumerator ForfeitSequence(TeamSide winnerSide)
    {
        yield return new WaitForSeconds(0.5f);
        ForceEndGame(winnerSide);
    }

    public void StartThrowIn(TeamSide side) 
    {
        StartCoroutine(ThrowInSequence(side));
    }

    private IEnumerator ThrowInSequence(TeamSide side)
    {
        AudioManager.Instance.PlaySfx("sfx-whistle_single");
        yield return new WaitForSeconds(0.5f);
        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.ThrowIn, side);
    }

    public void StartCornerKick(TeamSide side) 
    {
        StartCoroutine(CornerKickSequence(side));
    }

    private IEnumerator CornerKickSequence(TeamSide side)
    {
        AudioManager.Instance.PlaySfx("sfx-whistle_single");
        yield return new WaitForSeconds(0.5f);
        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.CornerKick, side);
    }

    public void StartOffside(TeamSide side) 
    {
        StartCoroutine(OffsideSequence(side));
    }

    private IEnumerator OffsideSequence(TeamSide side)
    {
        AudioManager.Instance.PlaySfx("sfx-whistle_single");

        BattleUIManager.Instance.SetMessageActive(MessageType.Offside, true);
        yield return new WaitForSeconds(0.5f);
        BattleUIManager.Instance.SetMessageActive(MessageType.Offside, false);

        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.FreeKickIndirect, side);
    }

    public void StartGoalKick(TeamSide side) 
    {
        StartCoroutine(GoalKickSequence(side));
    }

    private IEnumerator GoalKickSequence(TeamSide side)
    {
        AudioManager.Instance.PlaySfx("sfx-whistle_single");
        yield return new WaitForSeconds(0.5f);
        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.GoalKick, side);
    }
    #endregion

    #region Team and Ball
    private void HandleAllCharactersReady()
    {
        BattleEvents.RaiseBattleStart();
        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.Kickoff, TeamSide.Home);
    }

    private void PopulateTeamWithCharacters(Team team, int teamSize)
    {
        team.ClearCharacterEntities(currentType);
        team.ClearCharacters(currentType);

        TeamSide side = team.TeamSide;

        if (team.IsCustomLoadout)
            PopulateTeamWithCharactersFromLoadout(team, teamSize, side);
        else
            PopulateTeamWithCharactersFromData(team, teamSize);
    }

    // TODO refactor this to create 11 entity and 15 data
    private void PopulateTeamWithCharactersFromData(Team team, int teamSize)
    {
        for (int i = 0; i < teamSize; i++)
        {
            int characterIndex = i;
            BattleCharacterManager.Instance.GetPooledCharacter((character) =>
            {
                if (character != null && characterIndex < team.GetCharacterDataList(currentType).Count)
                {
                    CharacterData characterData = team.GetCharacterDataList(currentType)[characterIndex]; 
                    character.Initialize(characterData);

                    character.SetLevel(character.MaxLevel);
                    character.ForceMaxEvolutionOnEquippedMoves();
                    BattleCharacterManager.Instance.AssignCharacterToTeamBattle(character, team, characterIndex);
                    character.gameObject.name = character.CharacterId;
                    team.GetCharacterEntities(currentType).Add(character);
                    team.GetCharacters(currentType).Add(character.Character);
                
                    charactersReady++;
                    if (charactersReady >= charactersReadyMax)
                        BattleEvents.RaiseAllCharactersReady();
                }
            });
        }
    }

    // TODO refactor this to create 11 entity and 15 data
    private void PopulateTeamWithCharactersFromLoadout(Team team, int teamSize, TeamSide side)
    {
        for (int i = 0; i < teamSize; i++)
        {
            int characterIndex = i;
            BattleCharacterManager.Instance.GetPooledCharacter((character) =>
            {
                if (character != null && characterIndex < team.GetCharacterGuids(currentType).Count)
                {
                    Character characterObject = CharacterManager.Instance.GetCharacter(team.GetCharacterGuids(CurrentType)[characterIndex]);
                    character.Initialize(null, characterObject); //Initialize the CharacterEntityBattle with a characterObject

                    character.CalculateSpeed();
                    BattleCharacterManager.Instance.AssignCharacterToTeamBattle(character, team, characterIndex);
                    character.gameObject.name = character.CharacterId;
                    team.GetCharacterEntities(currentType).Add(character);
                    team.GetCharacters(currentType).Add(character.Character);
                
                    charactersReady++;
                    if (charactersReady >= charactersReadyMax)
                        BattleEvents.RaiseAllCharactersReady();
                }
            });
        }
    }

    public void ResetDefaultPositions()
    {
        AudioManager.Instance.PlayBgm("bgm-battle_crimson");
        ResetPlayerPositions();
        BattleBallManager.Instance.ResetBallPosition();
    }

    public void ResetPlayerPositions()
    {
        foreach (Team team in Teams.Values) 
        {
            foreach (var character in team.GetCharacterEntities(currentType))
            {
                BattleCharacterManager.Instance.ResetCharacterPosition(character);
            }
        }
    }
    #endregion
}
