using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;
using Simulation.Enums.Input;

public class DeadBallManager : MonoBehaviour
{
    #region Fields
    public static DeadBallManager Instance;

    private IDeadBallHandler currentHandler;
    private DeadBallPositionConfig positionConfig;
    private DeadBallCharacterSelector characterSelector;
    private DeadBallPositionUtils positionUtils;
    private TeamReadiness teamReadiness;
    private bool isFirstKickoff = true;
    private Team offenseTeam;
    private Team defenseTeam;
    private TeamSide offenseSide;
    private TeamSide defenseSide;
    private Vector3 cachedBallPosition;
    private Vector3 defaultPositionKickoffKicker;
    private Dictionary<TeamSide, Vector3> defaultPositionKickoffReceiver;

    public DeadBallPositionConfig PositionConfig => positionConfig;
    public DeadBallCharacterSelector CharacterSelector => characterSelector;
    public DeadBallPositionUtils PositionUtils => positionUtils;
    public TeamReadiness TeamReadiness => teamReadiness;
    public DeadBallState DeadBallState { get; private set; }
    public DeadBallType DeadBallType { get; private set; }
    public bool IsFirstKickoff => isFirstKickoff;
    public Vector3 CachedBallPosition => cachedBallPosition;
    public Team OffenseTeam => offenseTeam;
    public Team DefenseTeam => defenseTeam;
    public TeamSide OffenseSide => offenseSide;
    public TeamSide DefenseSide => defenseSide;

    #endregion

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

        positionConfig = new DeadBallPositionConfig();
        characterSelector = new DeadBallCharacterSelector(this);
        positionUtils = new DeadBallPositionUtils(this);
        teamReadiness = new TeamReadiness();

        defaultPositionKickoffKicker = new Vector3(0, 0.34f, 0);
        defaultPositionKickoffReceiver = new Dictionary<TeamSide, Vector3>
        {
            { TeamSide.Home, new Vector3(-1f, 0.34f, -0.1f) },
            { TeamSide.Away, new Vector3(1f, 0.34f, 0.1f) }
        };

    }

    #endregion

    #region Interface

    public void StartDeadBall(DeadBallType type, TeamSide teamSide)
    {
        BattleManager.Instance.Freeze();
        BattleManager.Instance.SetBattlePhase(BattlePhase.DeadBall);

        teamReadiness.Reset();

        DeadBallType = type;
        currentHandler = DeadBallFactory.Create(type);
        DeadBallState = DeadBallState.Setup;
        ResolveOffenseDefense(teamSide);

        currentHandler.Setup(teamSide);
    }

    private void Update()
    {
        if (currentHandler == null) return;

        //handleSharedInput Menu formation and change characters

        currentHandler.HandleInput();

        if (DeadBallState == DeadBallState.WaitingForReady && currentHandler.IsReady)
            Execute();
    }

    private void Execute()
    {
        DeadBallState = DeadBallState.Executing;
        currentHandler.Execute();
        Finish();
    }

    private void Finish()
    {
        DeadBallState = DeadBallState.Finished;
        BattleManager.Instance.Unfreeze();
        BattleManager.Instance.SetBattlePhase(BattlePhase.Battle);
        currentHandler = null;
    }

    #endregion

    #region Helpers

    public bool IsDeadBallInProgress => currentHandler != null && BattleManager.Instance.CurrentPhase == BattlePhase.DeadBall;
    public bool MarkFirstKickoffComplete() => isFirstKickoff = false;
    public Vector3 GetDefaultPositionKickoffKicker() => defaultPositionKickoffKicker;
    public Vector3 GetDefaultPositionKickoffReceive(TeamSide teamSide) => defaultPositionKickoffReceiver[teamSide];
    public void SetBallPosition(Vector3 ballPosition) => cachedBallPosition = ballPosition;
    public void SetState(DeadBallState state) => DeadBallState = state;

    private void ResolveOffenseDefense(TeamSide offenseSide)
    {
        this.offenseSide = offenseSide;
        defenseSide = offenseSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;

        offenseTeam = BattleManager.Instance.Teams[OffenseSide];
        defenseTeam = BattleManager.Instance.Teams[DefenseSide];
    }

    public bool IsOffense(TeamSide side) => side == offenseSide;
    public bool IsDefense(TeamSide side) => side == defenseSide;
    public bool IsUserOffense => 
        currentHandler != null &&
        DeadBallState == DeadBallState.WaitingForReady &&
        offenseSide == BattleManager.Instance.GetUserSide();
    public bool IsUserDefense => 
        currentHandler != null &&
        DeadBallState == DeadBallState.WaitingForReady &&
        defenseSide == BattleManager.Instance.GetUserSide();
    public bool IsUserTakingPenalty => 
        currentHandler != null &&
        DeadBallState == DeadBallState.WaitingForReady &&
        DeadBallType == DeadBallType.Penalty &&
        offenseSide == BattleManager.Instance.GetUserSide();

    public TeamSide GetRestartTeamSide() 
    {
        if (PossessionManager.Instance.CurrentCharacter == null)
            return PossessionManager.Instance.LastCharacter.TeamSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;
        else 
            return PossessionManager.Instance.CurrentCharacter.TeamSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;
    }

    // tell apart corner kick and goal kick when the ball crosses the end line
    public bool IsCornerKick(TeamSide teamSide)
    {
        BoundPlacement boundPlacement = positionUtils.GetBallEndPlacement(cachedBallPosition);

        if (teamSide == TeamSide.Away && boundPlacement == BoundPlacement.Bottom)
            return true;

        if (teamSide == TeamSide.Home && boundPlacement == BoundPlacement.Top)
            return true;

        return false;
    }

    #endregion
}
