using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;
using Simulation.Enums.Duel;

public class CharacterComponentAI : MonoBehaviour
{
    private Character character;

    // ============================
    // CONFIG CONSTANTS
    // ============================

    [Header("General AI Distances")]
    private const float ATTACK_DISTANCE = 1f;
    private const float DEFEND_DISTANCE = 0.4f;
    private const float CLOSE_DISTANCE_OPP_GOAL = 1.5f;
    private const float BALL_TIME_AHEAD = 0.9f;

    [Header("Passing Logic")]
    private const float MIN_PASS_DISTANCE = 1f;
    private const float MAX_PASS_DISTANCE = 5f;
    private const float REQUIRED_FORWARD_GAIN = 1.5f;
    private const float PASS_BLOCK_ANGLE_THRESHOLD = 0.8f;
    private const float PASS_BLOCK_DISTANCE = 0.4f;
    private const float PASS_LOOP_COOLDOWN = 2f;
    private const float MIN_PASS_RETURN_DISTANCE = 1.5f;

    [Header("Decision & Movement Settings")]
    private const float ROTATION_SPEED = 12f;
    private const float EASY_MIN_DELAY = 0.1f;
    private const float EASY_MAX_DELAY = 0.2f;
    private const float NORMAL_MIN_DELAY = 0.0f;
    private const float NORMAL_MAX_DELAY = 0.2f;
    private const float HARD_MIN_DELAY = 0.0f;
    private const float HARD_MAX_DELAY = 0.1f;

    [Header("Field & Formation")]
    private const float FIELD_HALF_WIDTH = 6f;
    private const float SIDELINE_BUFFER = 2f;
    private const float MIN_SPACING_BETWEEN_MATES = 1.5f;

    [Header("Keeper Settings")]
    private const float KEEPER_INTERCEPT_RANGE = 3f;
    private const float KEEPER_STANDING_OFFSET = 0.5f;

    [Header("Defensive Line Limits")]
    private const float HOME_DEFENSIVE_MIN_Z = -6f;
    private const float HOME_DEFENSIVE_MAX_Z = -1f;
    private const float AWAY_DEFENSIVE_MIN_Z = 1f;
    private const float AWAY_DEFENSIVE_MAX_Z = 6f;

    [Header("AI Traits Defaults")]
    private const float DEFAULT_AWARENESS = 0.75f;
    private const float DEFAULT_CONFIDENCE = 0.6f;

    // ============================
    // FIELDS
    // ============================

    [SerializeField] private bool isEnemyAI;
    [SerializeField] private bool isAIEnabled = false;  
    [SerializeField] private AIDifficulty difficulty;
    [SerializeField] private AIState currentState = AIState.Idle;

    public bool IsEnemyAI => isEnemyAI;
    public bool IsAIEnabled => isAIEnabled;
    public AIDifficulty AIDifficulty => difficulty;
    public AIState AIState => currentState;

    private Ball ball;
    private Goal ownGoal;
    private Goal opponentGoal;
    private List<Character> teammates;
    private List<Character> opponents;
    private float closeDistanceBall;
    private Character lastPassReceiver;
    private float lastPassTime = -100f;
    private float nextDecisionTime = 0f;
    private float minDecisionDelay;
    private float maxDecisionDelay;
    private float confidence = DEFAULT_CONFIDENCE;

    // ============================
    // INITIALIZATION
    // ============================

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
        this.difficulty = AIDifficulty.Hard;
        InitializeDecisionDelay();
    }

    private void InitializeDistances(Position position)
    {
        switch (position)
        {
            case Position.GK:
                closeDistanceBall = 0.5f;
                break;
            case Position.DF:
                closeDistanceBall = 2f;
                break;
            default:
                closeDistanceBall = 4f;
                break;
        }
    }

    private void InitializeDecisionDelay()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                minDecisionDelay = EASY_MIN_DELAY;
                maxDecisionDelay = EASY_MAX_DELAY;
                break;
            case AIDifficulty.Normal:
                minDecisionDelay = NORMAL_MIN_DELAY;
                maxDecisionDelay = NORMAL_MAX_DELAY;
                break;
            case AIDifficulty.Hard:
                minDecisionDelay = HARD_MIN_DELAY;
                maxDecisionDelay = HARD_MAX_DELAY;
                break;
        }
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;    
        BallEvents.OnBallSpawned += HandleBallSpawned;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
        BallEvents.OnBallSpawned -= HandleBallSpawned;
    }

    private void HandleAssignCharacterToTeamBattle(Character character, Team team, FormationCoord formationCoord)
    {
        if (this.character == character)
        {
            isEnemyAI = team.TeamSide == TeamSide.Away;
            ownGoal = GoalManager.Instance.GetOwnGoal(this.character);
            opponentGoal = GoalManager.Instance.GetOpponentGoal(this.character);
            teammates = character.GetTeammates();
            opponents = character.GetOpponents();
            InitializeDistances(formationCoord.Position);
            isAIEnabled = true;
        }
    }

    private void HandleBallSpawned(Ball ball) => this.ball = ball;

    public void EnableAI() => isAIEnabled = true;
    public void EnableAI(bool isAIEnabled) => this.isAIEnabled = isAIEnabled;
    public void DisableAI() => isAIEnabled = false;

    // ============================
    // UPDATE LOGIC
    // ============================

    private void Update()
    {
        if (character.IsControlled || !isAIEnabled || BattleManager.Instance.IsTimeFrozen)
            return;

        if (Time.time >= nextDecisionTime)
        {
            nextDecisionTime = Time.time + Random.Range(minDecisionDelay, maxDecisionDelay);
            UpdateCurrentAIState();
        }

        ExecuteCurrentAIState();
    }

    private void UpdateCurrentAIState()
    {
        if (!character.CanMove())
        {
            currentState = AIState.Idle;
            return;
        }

        float nearestOpponentDist = GetNearestOpponentDistanceTo(character);

        if (character.HasBall())
        {
            if (character.CanShoot())
                currentState = AIState.Shoot;
            else if ((Random.value > confidence && HasOpenTeammate()) || character.IsKeeper)
                currentState = AIState.Pass;
            else
                currentState = AIState.Dribble;
            return;
        }

        if (character.IsKeeper)
        {
            currentState = AIState.Keeper;
            return;
        }

        if (ball && ball.IsTraveling)
        {
            float distToBall = Vector3.Distance(character.transform.position, ball.transform.position);
            if (distToBall <= closeDistanceBall)
            {
                currentState = AIState.Combo;
                return;
            }
        }

        if (ball && ball.IsFree())
        {
            float distToBall = Vector3.Distance(character.transform.position, ball.transform.position);
            if (distToBall <= closeDistanceBall)
            {
                currentState = AIState.ChaseBall;
                return;
            }
        }

        if (OpponentHasBall())
        {
            if (nearestOpponentDist < ATTACK_DISTANCE * 3f)
                currentState = AIState.Defend;
            else 
                currentState = AIState.Mark;

            return;
        }

        currentState = AIState.Support;
    }

    private float GetNearestOpponentDistanceTo(Character character)
    {
        float nearestDist = float.MaxValue;
        foreach (Character opponent in opponents)
        {
            if (opponent == null || !opponent.CanMove()) continue;
            float dist = Vector3.Distance(character.transform.position, opponent.transform.position);
            if (dist < nearestDist) nearestDist = dist;
        }
        return nearestDist;
    }

    private Character GetNearestOpponentTo(Character character)
    {
        Character nearestOpponent = null;
        float nearestDist = float.MaxValue;

        foreach (Character opponent in opponents)
        {
            if (opponent == null || !opponent.CanMove()) continue;
            float dist = Vector3.Distance(character.transform.position, opponent.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestOpponent = opponent;
            }
        }
        return nearestOpponent;
    }

    private bool HasOpenTeammate()
    {
        Character openTeammate = null;
        float bestScore = float.MinValue;

        foreach (Character teammate in teammates)
        {
            if (teammate == null || teammate == character || !teammate.CanMove() || teammate.IsKeeper)
                continue;

            if (teammate == lastPassReceiver && Time.time - lastPassTime < PASS_LOOP_COOLDOWN)
                continue;

            float distance = Vector3.Distance(character.transform.position, teammate.transform.position);
            if (distance < MIN_PASS_DISTANCE || distance > MAX_PASS_DISTANCE)
                continue;

            float myDistToGoal = GoalManager.Instance.GetDistanceToOpponentGoalZ(character);
            float mateDistToGoal = GoalManager.Instance.GetDistanceToOpponentGoalZ(teammate);
            float forwardGain = myDistToGoal - mateDistToGoal;
            if (forwardGain < REQUIRED_FORWARD_GAIN)
                continue;

            bool blocked = false;
            foreach (Character opponent in opponents)
            {
                if (opponent == null || !opponent.CanMove()) continue;

                Vector3 passDir = (teammate.transform.position - character.transform.position).normalized;
                Vector3 toOpponent = (opponent.transform.position - character.transform.position);
                float proj = Vector3.Dot(passDir, toOpponent.normalized);
                if (proj > PASS_BLOCK_ANGLE_THRESHOLD)
                {
                    float distToLine = Vector3.Cross(passDir, toOpponent).magnitude;
                    if (distToLine < PASS_BLOCK_DISTANCE && toOpponent.magnitude < distance)
                    {
                        blocked = true;
                        break;
                    }
                }
            }

            if (blocked) continue;

            float forwardScore = myDistToGoal - mateDistToGoal;
            float opennessScore = 1f / (distance + 0.1f);
            float totalScore = forwardScore * 1.5f + opennessScore;

            if (totalScore > bestScore)
            {
                bestScore = totalScore;
                openTeammate = teammate;
            }
        }
        return openTeammate != null;
    }

    private bool OpponentHasBall()
    {
        Character otherCharacter = PossessionManager.Instance.CurrentCharacter;
        return otherCharacter && !otherCharacter.IsSameTeam(character);
    }

    // ============================
    // STATE EXECUTION
    // ============================

    private void ExecuteCurrentAIState()
    {
        switch (currentState)
        {
            case AIState.Idle: break;
            case AIState.ChaseBall: ActChaseBall(); break;
            case AIState.Pass: ActPass(); break;
            case AIState.Shoot: ActShoot(); break;
            case AIState.Defend: ActDefend(); break;
            case AIState.Keeper: ActKeeper(); break;
            case AIState.Combo: ActCombo(); break;
            case AIState.Dribble: ActDribble(); break;
            case AIState.Support: ActSupport(); break;
            case AIState.Mark: ActMark(); break;
        }
    }

    private void ActKeeper()
    {
        if (!character.IsKeeper) return;

        float distToBall = Vector3.Distance(character.transform.position, ball.transform.position);

        if (ball && distToBall < KEEPER_INTERCEPT_RANGE && ball.IsFree() && !ball.IsTraveling)
        {
            MoveTowards(PredictBallFuturePosition(0.3f));
        }
        else
        {
            Vector3 centerGoal = ownGoal.transform.position + Vector3.forward * (isEnemyAI ? KEEPER_STANDING_OFFSET : -KEEPER_STANDING_OFFSET);
            MoveTowards(centerGoal);
        }
    }

    private Vector3 PredictBallFuturePosition(float timeAhead) =>
        ball.transform.position + ball.GetVelocity() * timeAhead;

    private void ActChaseBall()
    {
        if (!ball) return;
        if (!ball.IsFree() && PossessionManager.Instance.CurrentCharacter != null) return;

        Character closestCharacter = CharacterChangeControlManager.Instance.GetClosestTeammateToBall(character, true);
        if (character != closestCharacter) return;

        float distToBall = Vector3.Distance(transform.position, ball.transform.position);
        Vector3 predicted = PredictBallFuturePosition(BALL_TIME_AHEAD * Mathf.Clamp01(distToBall / 10f));
        MoveTowards(predicted);
    }

    private void ActCombo() => ActChaseBall();

    private void ActDribble()
    {
        if (!character.HasBall()) return;

        Vector3 goalDir = (opponentGoal.transform.position - character.transform.position).normalized;
        Character nearestOpponent = GetNearestOpponentTo(character);
        Vector3 dodge = Vector3.zero;

        if (nearestOpponent)
        {
            float oppDist = Vector3.Distance(character.transform.position, nearestOpponent.transform.position);
            if (oppDist < 2f)
            {
                dodge = Vector3.Cross(Vector3.up, (nearestOpponent.transform.position - character.transform.position).normalized) * 0.5f;
            }
        }

        Vector3 target = character.transform.position + (goalDir + dodge).normalized;
        MoveTowards(target);
    }

    private Vector3 ClampDefensiveLine(Vector3 target)
    {
        float minZ, maxZ;
        if (isEnemyAI)
        {
            minZ = AWAY_DEFENSIVE_MIN_Z;
            maxZ = AWAY_DEFENSIVE_MAX_Z;
        }
        else
        {
            minZ = HOME_DEFENSIVE_MIN_Z;
            maxZ = HOME_DEFENSIVE_MAX_Z;
        }
        target.z = Mathf.Clamp(target.z, minZ, maxZ);
        return target;
    }

    private void ActSupport()
    {
        if (character.HasBall()) return;

        Character ballHolder = PossessionManager.Instance.CurrentCharacter;
        if (!ballHolder || !ballHolder.IsSameTeam(character))
        {
            MoveTowards(character.FormationCoord.DefaultPosition);
            return;
        }

        float maxSideOffset = FIELD_HALF_WIDTH - SIDELINE_BUFFER;

        Vector3 toGoal = (opponentGoal.transform.position - ballHolder.transform.position).normalized;
        Vector3 lateral = Vector3.Cross(Vector3.up, toGoal);

        float forwardOffset = 0f;
        float lateralBias = character.FormationCoord.DefaultPosition.x;

        switch (character.FormationCoord.Position)
        {
            case Position.GK:
                MoveTowards(ownGoal.transform.position);
                return;
            case Position.DF:
                forwardOffset = -6f;
                break;
            case Position.MF:
                forwardOffset = Random.Range(-2f, 1f);
                break;
            case Position.FW:
                forwardOffset = Random.Range(2f, 5f);
                break;
        }

        Vector3 supportTarget = ballHolder.transform.position + toGoal * forwardOffset;
        float desiredX = Mathf.Lerp(ballHolder.transform.position.x + lateralBias, character.FormationCoord.DefaultPosition.x, 0.5f);

        supportTarget.x = Mathf.Clamp(desiredX, -maxSideOffset, maxSideOffset);

        foreach (Character mate in teammates)
        {
            if (mate == character || !mate.CanMove()) continue;
            float dist = Vector3.Distance(character.transform.position, mate.transform.position);
            if (dist < MIN_SPACING_BETWEEN_MATES)
            {
                Vector3 sepDir = (character.transform.position - mate.transform.position).normalized;
                supportTarget += sepDir * (MIN_SPACING_BETWEEN_MATES - dist) * 0.5f;
            }
        }

        if (character.FormationCoord.Position == Position.DF)
            supportTarget = ClampDefensiveLine(supportTarget);

        MoveTowards(supportTarget);
    }

    private void ActMark()
    {
        Character ballHolder = PossessionManager.Instance.CurrentCharacter;
        if (!ballHolder || ballHolder.IsSameTeam(character))
        {
            ActDefend();
            return;
        }

        Character markTarget = ballHolder;
        float bestDist = float.MaxValue;
        foreach (var opp in opponents)
        {
            if (opp == null || !opp.CanMove()) continue;
            float dist = Vector3.Distance(character.transform.position, opp.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                markTarget = opp;
            }
        }

        Vector3 toGoal = (ownGoal.transform.position - markTarget.transform.position).normalized;
        Vector3 markPos = markTarget.transform.position + toGoal * REQUIRED_FORWARD_GAIN;
        MoveTowards(markPos);
    }

    private void ActDefend()
    {
        if (character.IsKeeper)
        {
            ActKeeper();
            return;
        }

        Character opponent = PossessionManager.Instance.CurrentCharacter;
        bool isOpponentValid = opponent && !opponent.IsSameTeam(character);

        if (!isOpponentValid)
        {
            opponent = GetNearestOpponentTo(character);
            if (!opponent)
            {
                MoveTowards(character.FormationCoord.DefaultPosition);
                return;
            }
        }

        Character closestCharacter = CharacterChangeControlManager.Instance.GetClosestTeammateToBall(character, true);
        if (character == closestCharacter)
        {
            MoveTowards(opponent.transform.position);
        }
        else
        {
            MoveTowards(character.FormationCoord.DefaultPosition);
        }
    }

    private void MoveTowards(Vector3 target)
    {
        if (!character.CanMove()) return;
        float speed = character.GetMovementSpeed();

        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;
        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ROTATION_SPEED * Time.deltaTime);

        Vector3 translation = direction * speed * Time.deltaTime;
        transform.Translate(translation, Space.World);
        transform.position = BoundManager.Instance.ClampCharacter(transform.position);  
    }

    private void ActPass()
    {
        Character teammate = GetBestPassTeammate();
        if (!teammate) return;

        character.KickBallTo(teammate.transform.position);
        lastPassReceiver = teammate;
        lastPassTime = Time.time;
    }

    private Character GetBestPassTeammate()
    {
        Character best = null;
        float bestScore = float.MinValue;

        foreach (var mate in teammates)
        {
            if (mate == character || !mate.CanMove()) continue;

            float angleScore = Vector3.Dot(
                (mate.transform.position - character.transform.position).normalized,
                (opponentGoal.transform.position - character.transform.position).normalized);

            float oppDistPenalty = -GetNearestOpponentDistanceTo(mate) * 0.5f;
            float distToGoal = GoalManager.Instance.GetDistanceToOpponentGoalZ(mate);

            float score = angleScore * 2 + (10f - oppDistPenalty) - distToGoal;

            if (score > bestScore)
            {
                bestScore = score;
                best = mate;
            }
        }
        return best;
    }

    private void ActShoot()
    {
        if (!DuelManager.Instance.IsResolved)
           return;

        bool isDirect = false;
        DuelManager.Instance.StartShootDuel(character, isDirect);
    }

    // ============================
    // DUEL AI
    // ============================

    public DuelCommand GetCommandByCategory(Category category)
    {
        if (DuelManager.Instance.IsKeeperDuel && category == Category.Dribble)
            return GetRegularCommand();

        switch (difficulty)
        {
            case AIDifficulty.Easy:
                return GetRegularCommand();
            case AIDifficulty.Normal:
                if (Random.value < 0.4f && character.HasAffordableMoveWithCategory(category))
                    return DuelCommand.Move;
                return GetRegularCommand();
            case AIDifficulty.Hard:
                return character.HasAffordableMoveWithCategory(category)
                    ? DuelCommand.Move
                    : GetRegularCommand();
            default:
                return DuelCommand.Melee;
        }
    }

    private DuelCommand GetRegularCommand() =>
        character.GetBattleStat(Stat.Body) > character.GetBattleStat(Stat.Control)
            ? DuelCommand.Melee
            : DuelCommand.Ranged;

    public Move GetMoveByCommandAndCategory(DuelCommand command, Category category)
    {
        if (command != DuelCommand.Move) return null;
        return (difficulty == AIDifficulty.Normal)
            ? character.GetRandomAffordableMoveByCategory(category)
            : character.GetStrongestAffordableMoveByCategory(category);
    }
}
