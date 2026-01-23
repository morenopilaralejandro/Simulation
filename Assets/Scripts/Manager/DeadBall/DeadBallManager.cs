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
    private bool isFirstKickoff = true;
    private Dictionary<TeamSide, bool> isTeamReady;
    private Team offenseTeam;
    private Team defenseTeam;
    private TeamSide offenseSide;
    private TeamSide defenseSide;

    private Vector3 cachedBallPosition;
    private Vector3 defaultPositionKickoffKicker;
    private Dictionary<TeamSide, Vector3> defaultPositionKickoffReceiver;

    public DeadBallState DeadBallState { get; private set; }
    public DeadBallType DeadBallType { get; private set; }
    public bool IsFirstKickoff => isFirstKickoff;
    public bool AreBothTeamsReady => isTeamReady[TeamSide.Home] && isTeamReady[TeamSide.Away];
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

        defaultPositionKickoffKicker = new Vector3(0, 0.34f, 0);
        defaultPositionKickoffReceiver = new Dictionary<TeamSide, Vector3>
        {
            { TeamSide.Home, new Vector3(-1f, 0.34f, -0.1f) },
            { TeamSide.Away, new Vector3(1f, 0.34f, 0.1f) }
        };

        isTeamReady = new Dictionary<TeamSide, bool>
        {
            { TeamSide.Home, false },
            { TeamSide.Away, false }
        };
    }

    #endregion

    #region Interface

    public void StartDeadBall(DeadBallType type, TeamSide teamSide)
    {
        BattleManager.Instance.Freeze();
        BattleManager.Instance.SetBattlePhase(BattlePhase.DeadBall);

        ResetTeamReady();

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

    public bool SetIsFirstKickoff(bool isFirst) => isFirstKickoff = isFirst;
    public Vector3 GetDefaultPositionKickoffKicker() => defaultPositionKickoffKicker;
    public Vector3 GetDefaultPositionKickoffReceive(TeamSide teamSide) => defaultPositionKickoffReceiver[teamSide];
    public void SetTeamReady(TeamSide teamSide) => isTeamReady[teamSide] = true;
    public void SetUserTeamReady() => isTeamReady[BattleManager.Instance.GetUserSide()] = true;

    public void SetBothTeamsReady() 
    {
        isTeamReady[TeamSide.Home] = true;
        isTeamReady[TeamSide.Away] = true;
    }

    private void ResetTeamReady() 
    {
        isTeamReady[TeamSide.Home] = false;
        isTeamReady[TeamSide.Away] = false;
    }

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
    public bool IsUserPenaltyOffense => 
        currentHandler != null &&
        DeadBallState == DeadBallState.WaitingForReady &&
        DeadBallType == DeadBallType.Penalty &&
        offenseSide == BattleManager.Instance.GetUserSide();

    #endregion
}
