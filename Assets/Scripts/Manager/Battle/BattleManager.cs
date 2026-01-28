using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;

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
    [SerializeField] private TimerHalf timerHalf;
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
        BattleEvents.OnAllCharactersReady += HandleAllCharactersReady;
    }

    void Start()
    {
        Freeze();
        SetTeamSize();
    }
    
    void Update()
    {
        if (isTimeFrozen) return;
            
        float timeScale = 15f;
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
        currentTeamSize = currentType == BattleType.Battle ? TeamManager.Instance.SizeBattle : TeamManager.Instance.SizeMiniBattle;
        charactersReadyMax = currentTeamSize*2;
    }
    #endregion

    #region StartBattle
    public void StartBattle()
    {
        Freeze();
        SetBattleType(BattleArgs.BattleType);
        //SetBattlePhase(BattlePhase.Battle);
        ResetBattle();

        BattleFieldManager.Instance.InitializeField();

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
        timerHalf = TimerHalf.First;
        BattleUIManager.Instance.UpdateTimerDisplay(timeCurrent);
        BattleUIManager.Instance.UpdateTimerHalfDisplay(timerHalf);
        if (currentType == BattleType.MiniBattle)
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

        BattleEvents.RaiseGoalScored(PossessionManager.Instance.LastCharacter);
        StartCoroutine(GoalSequence(goal.TeamSide));
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
        // Determine which side won
        int homeScore = scoreDict[TeamSide.Home];
        int awayScore = scoreDict[TeamSide.Away];

        TeamSide? winningSide = null;
        if (homeScore > awayScore)
            winningSide = TeamSide.Home;
        else if (awayScore > homeScore)
            winningSide = TeamSide.Away;
        else
            winningSide = null; // tie

        TeamSide userSide = GetUserSide();
        LogManager.Info($"[BattleManager] Game Ended — Home: {homeScore} Away: {awayScore}, User side: {userSide}, Winner: {winningSide}", this);

        SetBattlePhase(BattlePhase.End);
        BattleEvents.RaiseBattleEnd();
        SceneLoader.UnloadBattle();
        if (winningSide == userSide)
        {
            // User won
            SceneLoader.LoadBattleResults();
        }
        else
        {
            // User lost
            SceneLoader.LoadGameOver();
        }
    }

    private void ForceEndGame(TeamSide winnerSide)
    {
        if (currentPhase == BattlePhase.End) return;        

        SetBattlePhase(BattlePhase.End);
        BattleEvents.RaiseBattleEnd();
        SceneLoader.UnloadBattle();

        TeamSide userSide = GetUserSide();
        if (winnerSide == userSide)
            SceneLoader.LoadBattleResults();
        else
            SceneLoader.LoadGameOver();
    }

    public void ForfeitBattle()
    {
        LogManager.Info("[BattleManager] User forfeits the battle.", this);
        
        Freeze(); // Stop movement/time
        
        // Determine user’s side, so the opponent wins
        TeamSide userSide = GetUserSide();
        TeamSide opponentSide = (userSide == TeamSide.Home) ? TeamSide.Away : TeamSide.Home;

        // Optional: update UI message
        //BattleUIManager.Instance.SetMessageActive(MessageType.Forfeit, true);

        // Wait briefly before ending the game (optional)
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

    private IEnumerator TimeOverSequence()
    {
        float duration = 1.5f;

        //cancel lingering duel
        if (!DuelManager.Instance.IsResolved)
            Ball.CancelTravel();

        //Show message
        MessageType messageType = MessageType.TimeUp;
        if (currentType == BattleType.Battle)
            messageType =  
                timerHalf == TimerHalf.First ?
                MessageType.HalfTime :
                MessageType.FullTime;
        BattleUIManager.Instance.SetMessageActive(messageType, true);

        //play sound effect
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

        //resolve
        if (currentType == BattleType.MiniBattle ||
            timerHalf == TimerHalf.Second) 
        {
            EndGame();
        } else {
            StartSecondHalf();
        }

    }

    private IEnumerator ForfeitSequence(TeamSide winnerSide)
    {
        //AudioManager.Instance.PlaySfx("sfx-whistle_triple");
        yield return new WaitForSeconds(0.5f);

        // Hide message
        //BattleUIManager.Instance.SetMessageActive(MessageType.Forfeit, false);

        // Force end game — treat as loss for user
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

        // Hide message
        //BattleUIManager.Instance.SetMessageActive(MessageType.Forfeit, false);
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

        // Hide message
        //BattleUIManager.Instance.SetMessageActive(MessageType.Forfeit, false);
        ResetDefaultPositions();
        DeadBallManager.Instance.StartDeadBall(DeadBallType.CornerKick, side);
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
