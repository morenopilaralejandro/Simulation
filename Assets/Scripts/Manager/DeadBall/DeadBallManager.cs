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
    private bool isFirstKickoff = true;
    private Dictionary<TeamSide, bool> isTeamReady;
    private Team offenseTeam;
    private Team defenseTeam;
    private TeamSide offenseSide;
    private TeamSide defenseSide;

    private Vector3 cachedBallPosition;
    private Vector3 defaultPositionKickoffKicker;
    private Dictionary<TeamSide, Vector3> defaultPositionKickoffReceiver;

    public DeadBallPositionConfig PositionConfig => positionConfig;
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

        positionConfig = new DeadBallPositionConfig();
        positionConfig.Initialize();

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

    public bool MarkFirstKickoffComplete(bool isFirst) => isFirstKickoff = isFirst;
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

    public Character GetKickerCharacter(Team team)
    {
        Character nearestCharacter = null;
        List<Character> teammates = team.CharacterList;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < BattleManager.Instance.CurrentTeamSize; i++)
        {
            Character teammate = teammates[i];

            // Skip self unless includeSelf is true
            if (teammate.IsKeeper || (DeadBallType == DeadBallType.CornerKick && teammate.FormationCoord.Position == Position.FW))
                continue;
            float dist = Vector3.Distance(
                cachedBallPosition, 
                teammate.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestCharacter = teammate;
            }
        }

        return nearestCharacter;
    }

    public Character[] GetClosestCharacters(Team team, Character kicker)
    {
        int arrayLength = 3;
        Character[] closest = new Character[arrayLength];
        float[] distances = { Mathf.Infinity, Mathf.Infinity, Mathf.Infinity };

        foreach (Character teammate in team.CharacterList)
        {
            if (teammate == kicker || teammate.IsKeeper)
                continue;

            float dist = Vector3.Distance(cachedBallPosition, teammate.transform.position);

            for (int i = 0; i < arrayLength; i++)
            {
                if (dist < distances[i])
                {
                    // Shift down
                    for (int j = arrayLength - 1; j > i; j--)
                    {
                        distances[j] = distances[j - 1];
                        closest[j] = closest[j - 1];
                    }

                    distances[i] = dist;
                    closest[i] = teammate;
                    break;
                }
            }
        }

        return closest;
    }

    public CornerPlacement GetBallCornerPlacement(Vector3 ballPos)
    {
        bool isRight = ballPos.x > 0f;
        bool isBottom = ballPos.z < 0f;

        if (!isRight && !isBottom) return CornerPlacement.TopLeft;
        if (isRight && !isBottom)  return CornerPlacement.TopRight;
        if (!isRight && isBottom)  return CornerPlacement.BottomLeft;
        return CornerPlacement.BottomRight;
    }

    public Vector3 FlipPositionOnCorner(Vector3 basePos, CornerPlacement cornerPlacement)
    {
        Vector3 pos = basePos;

        switch (cornerPlacement)
        {
            case CornerPlacement.TopRight:
                pos.x *= -1f;
                break;

            case CornerPlacement.BottomLeft:
                pos.z *= -1f;
                break;

            case CornerPlacement.BottomRight:
                pos.x *= -1f;
                pos.z *= -1f;
                break;
        }

        return pos;
    }

    public BoundPlacement GetBallEndPlacement(Vector3 ballPos)
    {
        if (ballPos.z > 0f) 
            return BoundPlacement.Top;
        return BoundPlacement.Bottom;
    }

    public bool IsCornerKick(TeamSide teamSide)
    {
        BoundPlacement boundPlacement = GetBallEndPlacement(cachedBallPosition);

        if (teamSide == TeamSide.Away && boundPlacement == BoundPlacement.Bottom)
            return true;

        if (teamSide == TeamSide.Home && boundPlacement == BoundPlacement.Top)
            return true;

        return false;
    }

    // Invert index order for away team so formations mirror correctly
    private int GetTeamAdjustedIndex(int index, int length, TeamSide teamSide)
    {
        if (teamSide == TeamSide.Away)
            return length - 1 - index;
        return index;
    }

    public void SetCornerPositions(
        Character[] characters,
        Vector3[] basePositions,
        TeamSide teamSide,
        CornerPlacement cornerPlacement
    )
    {
        int count = Mathf.Min(characters.Length, basePositions.Length);

        for (int i = 0; i < count; i++)
        {
            int adjustedIndex = GetTeamAdjustedIndex(i, basePositions.Length, teamSide);

            Vector3 basePos = basePositions[adjustedIndex];
            Vector3 finalPos = FlipPositionOnCorner(basePos, cornerPlacement);

            characters[i].Teleport(finalPos);
        }
    }

    public int GetDefaultRecieverIndexInArray(Character[] array, Character characterKicker) 
    {
        if (characterKicker.IsEnemyAI)
            return array.Length - 1;
        else
            return 0;
    }

    #endregion
}
