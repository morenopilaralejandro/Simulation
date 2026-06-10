using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.DeadBall;
using Aremoreno.Enums.Kit;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    [Header("Battle")]
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

    [Header("Battle Subsystem")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject characterEntityBattlePrefab;

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

    // Win condition
    private WinCondition winCondition;
    public WinCondition WinCondition => winCondition;

    private int charactersReadyMax;
    private int charactersReady;

    private BattleManagerBall ballSystem;
    private BattleManagerCharacter characterSystem;
    private BattleManagerEffect effectSystem;
    private BattleManagerField fieldSystem;
    private BattleManagerTeam teamSystem;
    private BattleManagerWing wingSystem;

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

        ballSystem = new BattleManagerBall(ballPrefab);
        characterSystem = new BattleManagerCharacter(characterEntityBattlePrefab);
        effectSystem = new BattleManagerEffect();
        fieldSystem = new BattleManagerField();
        teamSystem = new BattleManagerTeam();
        wingSystem = new BattleManagerWing();

        teamSystem.Subscribe();
        wingSystem.Subscribe();

        Freeze();
        SetTeamSize();
    }
    
    void Update()
    {
        if (isTimeFrozen) return;
            
        timeCurrent += Time.deltaTime * timeScale;
        BattleUIManager.Instance.UpdateTimerDisplay(timeCurrent);
        
        if(Ball != null && !Ball.IsTraveling)
            CheckEndGame();
    }

    private void OnDestroy()
    {
        BattleEvents.OnAllCharactersReady -= HandleAllCharactersReady;

        if (teamSystem != null) teamSystem.Unsubscribe();
        if (teamSystem != null) wingSystem.Unsubscribe();
    }
    #endregion

    #region Generic
    public void Freeze()
    {
        //LogManager.Trace("[BattleManager] Freeze");
        isMovementFrozen = true;
        isTimeFrozen = true;
    }

    public void Unfreeze()
    {
        //LogManager.Trace("[BattleManager] Unfreeze");
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

        InitializeField();

        teamSystem.AssignTeamToSide(
            teamSystem.ResolveTeamForSide(TeamSide.Home), 
            TeamSide.Home);
        teamSystem.AssignTeamToSide(
            teamSystem.ResolveTeamForSide(TeamSide.Away), 
            TeamSide.Away);
        teamSystem.AssignVariants();

        foreach (Team team in Teams.Values)
        {
            _ = BattleUIManager.Instance.SetTeamAsync(team);
            PopulateTeamWithCharacters(team, CurrentTeamSize);
        }
    }

    private void ResetBattle()
    {
        teamSystem.Reset();
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

        CharacterEntityBattle scoringCharacter = null;

        if (PossessionManager.Instance.CurrentCharacter == null)
        {
            scoringCharacter = PossessionManager.Instance.LastCharacter;
        }
        else 
        {
            scoringCharacter = PossessionManager.Instance.CurrentCharacter;
            scoringCharacter.TryDeactivateWings(); //prevent goal by dash
        }

        BattleEvents.RaiseGoalScored(scoringCharacter);


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
        yield return new WaitForSeconds(0.5f);
        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.GoalKick, side);
    }
    #endregion

    #region Team and Ball
    private void HandleAllCharactersReady()
    {
        ResetDefaultPositions();
        BattleEvents.RaiseBattleStart(currentType);

        if (currentType == BattleType.Mini)
        {

            DeadBallManager.Instance.StartDeadBall(DeadBallType.Kickoff, TeamSide.Home);
        } else 
        {
            TeamEvents.RaiseTeamPreviewRequested();
        }
    }

    private void HandleTeamPreviewEnded()
    {
        DeadBallManager.Instance.StartDeadBall(DeadBallType.Kickoff, TeamSide.Home);
    }

    private void PopulateTeamWithCharacters(Team team, int teamSize)
    {
        team.ClearCharacterEntities(currentType);
        team.ClearCharacters(currentType);

        int rosterSize = currentType == BattleType.Full ? TeamManager.SIZE_MAX : teamSize;

        for (int i = 0; i < rosterSize; i++)
        {
            int characterIndex = i;
            bool needsEntity = i < teamSize;

            Character characterObject = AddCharacterToRoster(team, characterIndex);

            if (!needsEntity || characterObject == null) continue;

            characterSystem.GetPooledCharacter((characterEntity) =>
            {
                if (characterEntity == null) return;

                characterEntity.Initialize(null, characterObject);
                characterEntity.CalculateSpeed();
                characterSystem.AssignCharacterToTeamBattle(characterEntity, team, characterIndex);
                characterEntity.gameObject.name = characterEntity.CharacterId;
                team.GetCharacterEntities(currentType).Add(characterEntity);

                charactersReady++;
                if (charactersReady >= charactersReadyMax)
                    BattleEvents.RaiseAllCharactersReady();
            });
        }
    }

    private Character AddCharacterToRoster(Team team, int index)
    {
        Character characterObject = null;
        if (team.IsCustomLoadout) 
        {
            var guids = team.GetCharacterGuids(currentType);
            if (index >= guids.Count) return null;

            characterObject = CharacterManager.Instance.GetCharacter(guids[index]);
            TryInitializeFromLoadout(characterObject, team, index);
        }
        else 
        {
            var dataList = team.GetCharacterDataList(currentType);
            if (index >= dataList.Count) return null;

            characterObject = new Character(dataList[index]);
            TryInitializeFromData(characterObject, team, index);
        }
        team.GetCharacters(currentType).Add(characterObject);
        return characterObject;
    }

    private bool TryInitializeFromData(Character character, Team team, int index)
    {
        var dataList = team.GetCharacterDataList(currentType);
        if (index >= dataList.Count) return false;

        character.Initialize(dataList[index]);
        character.SetLevel(character.MaxLevel);
        character.TryEquipWingDefault();
        character.ScaleDifficultySystem();
        return true;
    }

    private bool TryInitializeFromLoadout(Character character, Team team, int index)
    {
        var guids = team.GetCharacterGuids(currentType);
        if (index >= guids.Count) return false;

        return true;
    }

    public void ResetDefaultPositions()
    {
        AudioManager.Instance.PlayBgm("bgm-battle_crimson");
        ResetPlayerPositions();
        ballSystem.ResetBallPosition();
    }

    public void ResetPlayerPositions()
    {
        foreach (Team team in Teams.Values) 
        {
            foreach (var character in team.GetCharacterEntities(currentType))
            {
                characterSystem.ResetCharacterPosition(character);
            }
        }
    }
    #endregion

    #region Events

    private void OnEnable()
    {
        TeamEvents.OnTeamPreviewEnded += HandleTeamPreviewEnded;
    }

    private void OnDisable()
    {
        TeamEvents.OnTeamPreviewEnded -= HandleTeamPreviewEnded;
    }

    #endregion

    #region API

    //ballSystem
    public Ball Ball => ballSystem.Ball; 
    public void RegisterSpawnPointBall(Transform spawner) => ballSystem.RegisterSpawnPoint(spawner);
    public void UnregisterSpawnPointBall() => ballSystem.UnregisterSpawnPoint();
    public void SpawnBall() => ballSystem.Spawn();
    public void ResetBallPosition() => ballSystem.ResetBallPosition();

    //characterSystem
    public void RegisterSpawnPointCharacter(Transform spawner) => characterSystem.RegisterSpawnPoint(spawner);
    public void UnregisterSpawnPointCharacter() => characterSystem.UnregisterSpawnPoint();
    public void GetPooledCharacter(Action<CharacterEntityBattle> onCharacterReady) => characterSystem.GetPooledCharacter(onCharacterReady);
    public void ReturnCharacterToPool(CharacterEntityBattle character) => characterSystem.ReturnCharacterToPool(character);
    public void AssignCharacterToTeamBattle(CharacterEntityBattle character, Team team, int characterIndex) => characterSystem.AssignCharacterToTeamBattle(character, team, characterIndex);
    public void ResetCharacterPosition(CharacterEntityBattle character) => characterSystem.ResetCharacterPosition(character);
    public void ClearCharacterPool() => characterSystem.ClearPool();

    //effectSystem
    public bool IsPlayingMove => effectSystem.IsPlayingMove;
    public bool IsPlayingWing => effectSystem.IsPlayingWing;
    public void RegisterSpawnPointEffect(BattleEffectSpawnPoint spawner) => effectSystem.RegisterSpawnPoint(spawner);
    public void UnregisterSpawnPointEffect() => effectSystem.UnregisterSpawnPoint();
    public void PlayDuelStartEffect(Transform originTransform) => effectSystem.PlayDuelStartEffect(originTransform);
    public void StopDuelStartEffect() => effectSystem.StopDuelStartEffect();
    public void PlayDuelWinEffect(Transform originTransform) => effectSystem.PlayDuelWinEffect(originTransform);
    public ParticleSystem GetMoveParticle(Element element) => effectSystem.GetMoveParticle(element);
    public async Task PlayMoveParticle(Move move, Vector3 position) => await effectSystem.PlayMoveParticle(move, position);
    public async Task PlayWingParticle(Wing wing, Vector3 position) => await effectSystem.PlayWingParticle(wing, position);

    //fieldSystem
    public void RegisterField(Field field) => fieldSystem.RegisterField(field);
    public void UnregisterField() => fieldSystem.UnregisterField();
    public void InitializeField() => fieldSystem.InitializeField();

    //teamSystem
    public Dictionary<TeamSide, Team> Teams => teamSystem.Teams;
    public void AssignTeamToSide(Team team, TeamSide teamSide) => teamSystem.AssignTeamToSide(team, teamSide);
    public void AssignVariantToTeam(Team team, Variant variant) => teamSystem.AssignVariantToTeam(team, variant);
    public void AssignVariants() => teamSystem.AssignVariants();
    public TeamSide GetUserSide() => teamSystem.GetUserSide();
    public void ResetTeamSystem() => teamSystem.Reset();
    public Team ResolveTeamForSide(TeamSide side) => teamSystem.ResolveTeamForSide(side);

    //wingSystem
    public void InitializeForBattleWingSystem() => wingSystem.InitializeForBattle();
    public bool CanActivateWings(TeamSide teamside) => wingSystem.CanActivateWings(teamside);

    //misc
    public GameObject InstantiateBall(GameObject prefabGo, Vector3 spawnPosition, Quaternion spawnRotation, Transform spawnPoint)
    {
        return Instantiate(prefabGo, spawnPosition, spawnRotation, spawnPoint);
    }
    public GameObject InstantiateCharacter(GameObject prefabGo, Transform spawnPoint) 
    {
        return Instantiate(prefabGo, spawnPoint);
    }
    public void DestroyGameObject(GameObject go)
    {
        Destroy(go);
    }

    //other
    public Dictionary<TeamSide, CharacterEntityBattle> TargetedCharacter => CharacterTargetManager.Instance.TargetedCharacter;
    public Dictionary<TeamSide, CharacterEntityBattle> ControlledCharacter => CharacterChangeControlManager.Instance.ControlledCharacter;

    #endregion
}
