using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;
using Simulation.Enums.Duel;

public class CharacterComponentAI : MonoBehaviour
{
    #region CONFIG CONSTANTS

    // Target locking
    private Vector3 currentMoveTarget;
    private float targetLockUntil;
    private const float TARGET_LOCK_TIME = 0.6f;
    private const float DEFENSIVE_TARGET_LOCK_TIME = 0.1f;
    private const float MARKING_TARGET_LOCK_TIME = 1.4f;

    // General AI distances
    private const float ATTACK_DISTANCE = 1f;
    private const float DEFEND_DISTANCE = 0.4f;
    private const float CLOSE_DISTANCE_OPP_GOAL = 1.5f;
    private const float BALL_TIME_AHEAD = 0.9f;

    // Passing logic
    private const float MIN_PASS_DISTANCE = 1f;
    private const float MAX_PASS_DISTANCE = 3f;
    private const float REQUIRED_FORWARD_GAIN = 1.5f;
    private const float PASS_BLOCK_ANGLE_THRESHOLD = 0.8f;
    private const float PASS_BLOCK_DISTANCE = 0.4f;
    private const float PASS_LOOP_COOLDOWN = 2f;
    private const float MIN_PASS_RETURN_DISTANCE = 1.5f;

    // Decision & movement
    private const float ROTATION_SPEED = 12f;
    private const float EASY_MIN_DELAY = 0.1f;
    private const float EASY_MAX_DELAY = 0.2f;
    private const float NORMAL_MIN_DELAY = 0.0f;
    private const float NORMAL_MAX_DELAY = 0.2f;
    private const float HARD_MIN_DELAY = 0.0f;
    private const float HARD_MAX_DELAY = 0.1f;

    // Field & formation
    private const float FIELD_HALF_WIDTH = 6f;
    private const float SIDELINE_BUFFER = 2f;
    private const float MIN_SPACING_BETWEEN_MATES = 1.5f;

    // Keeper
    private const float KEEPER_INTERCEPT_RANGE = 3f;
    private const float KEEPER_STANDING_OFFSET = 0.5f;
    private const float KEEPER_PREDICT_AHEAD = 0.3f;

    // Defensive line limits
    private const float HOME_DEFENSIVE_MIN_Z = -7.0f;
    private const float HOME_DEFENSIVE_MAX_Z = -4.0f;
    private const float AWAY_DEFENSIVE_MIN_Z =  4.0f;
    private const float AWAY_DEFENSIVE_MAX_Z =  7.0f;

    // AI traits defaults
    private const float DEFAULT_AWARENESS = 0.75f;
    private const float DEFAULT_CONFIDENCE = 0.6f;

    // Initialization distances by position
    private const float CLOSE_DISTANCE_GK = 0.5f;
    private const float CLOSE_DISTANCE_DF = 2f;
    private const float CLOSE_DISTANCE_OTHER = 4f;

    // Support offsets (formation)
    private const float SUPPORT_FORWARD_OFFSET_GK = 0f;
    private const float SUPPORT_FORWARD_OFFSET_DF = -6f;
    private const float SUPPORT_FORWARD_OFFSET_MF_MIN = -2f;
    private const float SUPPORT_FORWARD_OFFSET_MF_MAX = 1f;
    private const float SUPPORT_FORWARD_OFFSET_FW_MIN = 2f;
    private const float SUPPORT_FORWARD_OFFSET_FW_MAX = 5f;

    // Misc small values
    private const float OPENNESS_DISTANCE_EPS = 0.1f;
    private const float FORWARD_SCORE_WEIGHT = 1.5f;
    private const float ANGLE_SCORE_WEIGHT = 2f;
    private const float OPPONENT_DISTANCE_PENALTY_MULTIPLIER = 0.5f;
    private const float PASS_SCORE_BASE = 10f;

    // Ball prediction tuning
    private const float BALL_PREDICT_DISTANCE_SCALE = 10f;

    // Dribble behavior
    private const float OPPONENT_DODGE_DISTANCE = 2f;
    private const float OPPONENT_DODGE_DISTANCE_SQR = OPPONENT_DODGE_DISTANCE * OPPONENT_DODGE_DISTANCE;
    private const float DRIBBLE_DODGE_STRENGTH = 0.6f;
    private const float DRIBBLE_STEP_DISTANCE = 2f;

    // Support smoothing
    private const float SUPPORT_DESIREDX_LERP = 0.5f;
    private const float SEPARATION_ADJUST_FACTOR = 0.5f;
    private const float CACHE_UPDATE_THRESHOLD = 2f;
    private const float CACHE_LERP_FACTOR = 0.01f;

    // Marking & defending
    private const float MARK_OFFSET_DISTANCE = 1.2f;
    private const float DEFENDER_ADVANCE_TOLERANCE = 0.5f;
    private const float PRESS_BACKOFF_DISTANCE = 0.8f;
    private const float MARK_LOCK_TIME = 1.2f;
    private const float PRESSURE_DISTANCE = 1f;
    private const float DEFENDER_MAX_ADVANCE_HOME = -6f;
    private const float DEFENDER_MAX_ADVANCE_AWAY = 6f;
    private const float DEFENDER_ADVANCE_CLAMP_TOLERANCE = 2f;

    // Commit / tackle logic
    private float timeInCommitRange = 0f;
    private bool isCommitting = false;

    private const float COMMIT_DISTANCE = 1f;
    private const float COMMIT_TIME_REQUIRED = 0.25f;
    private const float COMMIT_FORCE_MULTIPLIER = 1.8f;

    // Movement physics
    private const float AI_ACCELERATION = 50f;
    private const float MIN_VELOCITY_SQR = 0.01f;
    private const float MIN_TARGET_DIST_SQR = 0.01f;

    // Cached / initial seed values
    private const float INIT_LAST_PASS_TIME = -100f;

    // Pass/score tuning (GetBestPassTeammate)
    private const float OPPONENT_DIST_PENALTY_BASE = 0.5f;

    // Duel defaults
    private const bool SHOOT_DIRECT_DEFAULT = false;
    private const bool SHOOT_LONG_DEFAULT = false;

    #endregion

    #region SERIALIZED & PROPERTIES

    [SerializeField] private Rigidbody rb;

    [SerializeField] private bool isEnemyAI;
    [SerializeField] private bool isAIEnabled = false;
    [SerializeField] private AIDifficulty difficulty;
    [SerializeField] private AIState currentState = AIState.Idle;
    [SerializeField] private bool isAutoBattleEnabled = false;

    public bool IsEnemyAI => isEnemyAI;
    public bool IsAIEnabled => isAIEnabled;
    public AIDifficulty AIDifficulty => difficulty;
    public AIState AIState => currentState;
    public bool IsAutoBattleEnabled => isAutoBattleEnabled;

    #endregion

    #region FIELDS

    private Character character;

    private Ball ball;
    private Goal ownGoal;
    private Goal opponentGoal;
    private List<Character> teammates;
    private List<Character> opponents;
    private float closeDistanceBall;
    private Character lastPassReceiver;
    private float lastPassTime = INIT_LAST_PASS_TIME;
    private float nextDecisionTime = 0f;
    private float minDecisionDelay;
    private float maxDecisionDelay;
    private float confidence = DEFAULT_CONFIDENCE;
    private float forwardOffset;
    private Vector3 cachedSupportTarget;

    #endregion

    #region INITIALIZATION

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
                closeDistanceBall = CLOSE_DISTANCE_GK;
                break;
            case Position.DF:
                closeDistanceBall = CLOSE_DISTANCE_DF;
                break;
            default:
                closeDistanceBall = CLOSE_DISTANCE_OTHER;
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

    private void InitializeSupportForwardOffset(Position position)
    {
        switch (position)
        {
            case Position.GK:
                forwardOffset = SUPPORT_FORWARD_OFFSET_GK;
                break;
            case Position.DF:
                forwardOffset = SUPPORT_FORWARD_OFFSET_DF;
                break;
            case Position.MF:
                forwardOffset = Random.Range(SUPPORT_FORWARD_OFFSET_MF_MIN, SUPPORT_FORWARD_OFFSET_MF_MAX);
                break;
            case Position.FW:
                forwardOffset = Random.Range(SUPPORT_FORWARD_OFFSET_FW_MIN, SUPPORT_FORWARD_OFFSET_FW_MAX);
                break;
        }
    }

    private void SetMoveTarget(Vector3 target)
    {
        if (Time.time < targetLockUntil)
            return;

        float lockTime = TARGET_LOCK_TIME;
        switch(currentState) 
        {
            case AIState.Defend:
                if(character.FormationCoord.Position == Position.DF)
                    lockTime = DEFENSIVE_TARGET_LOCK_TIME;
                else 
                    lockTime = TARGET_LOCK_TIME;
                break;
            case AIState.Mark:
                lockTime = MARKING_TARGET_LOCK_TIME;
                break;
            default:
                lockTime = TARGET_LOCK_TIME;
                break;
        }

        currentMoveTarget = target;
        targetLockUntil = Time.time + lockTime;
    }

    #endregion

    #region EVENTS

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;
        BallEvents.OnBallSpawned += HandleBallSpawned;
        SettingsEvents.OnAutoBattleToggled += HandleAutoBattleToggled;
        BallEvents.OnGained += HandleGained;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
        BallEvents.OnBallSpawned -= HandleBallSpawned;
        SettingsEvents.OnAutoBattleToggled -= HandleAutoBattleToggled;
        BallEvents.OnGained -= HandleGained;
    }

    private void HandleGained(Character gainedBy)
    {
        ClearDefensiveAssignments();
        ResetCommit();
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
            InitializeSupportForwardOffset(formationCoord.Position);
            isAIEnabled = true;
            isAutoBattleEnabled = SettingsManager.Instance.IsAutoBattleEnabled;
        }
    }

    private void HandleBallSpawned(Ball ball) => this.ball = ball;
    private void HandleAutoBattleToggled(bool enable) => isAutoBattleEnabled = enable;

    public void EnableAI() => isAIEnabled = true;
    public void EnableAI(bool isAIEnabled) => this.isAIEnabled = isAIEnabled;
    public void DisableAI() => isAIEnabled = false;

    #endregion

    #region UPDATE LOOP

    private void FixedUpdate()
    {
        if ((character.IsControlled && !character.IsAutoBattleEnabled) ||
            !isAIEnabled ||
            BattleManager.Instance.IsTimeFrozen)
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
        if (!character.CanMove() || character.IsStateLocked)
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
            currentState = AIState.Defend;
            return;
        }

        currentState = AIState.Support;
    }

    #endregion

    #region SENSING HELPERS

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
            float opennessScore = 1f / (distance + OPENNESS_DISTANCE_EPS);
            float totalScore = forwardScore * FORWARD_SCORE_WEIGHT + opennessScore;

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

    #endregion

    #region STATE EXECUTION

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

    #endregion

    #region ACTIONS - KEEPER / CHASE / DRIBBLE

    private void ActKeeper()
    {
        if (!ball) return;

        float distSqr = (transform.position - ball.transform.position).sqrMagnitude;

        if (ball.IsFree() && distSqr < KEEPER_INTERCEPT_RANGE * KEEPER_INTERCEPT_RANGE)
        {
            Vector3 predicted = PredictBallFuturePosition(KEEPER_PREDICT_AHEAD);
            SetMoveTarget(predicted);
            MoveTowards(currentMoveTarget);
        }
        else
        {
            SetMoveTarget(character.FormationCoord.DefaultPosition);
            MoveTowards(currentMoveTarget);
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
        Vector3 predicted = PredictBallFuturePosition(BALL_TIME_AHEAD * Mathf.Clamp01(distToBall / BALL_PREDICT_DISTANCE_SCALE));
        MoveTowards(predicted);
    }

    private void ActCombo() => ActChaseBall();

    private void ActDribble()
    {
        if (!character.HasBall()) return;

        Vector3 goalDir = (opponentGoal.transform.position - transform.position).normalized;
        Vector3 dodge = Vector3.zero;

        Character opp = GetNearestOpponentTo(character);
        if (opp)
        {
            float dist = (opp.transform.position - transform.position).sqrMagnitude;
            if (dist < OPPONENT_DODGE_DISTANCE_SQR)
                dodge = Vector3.Cross(Vector3.up, goalDir) * DRIBBLE_DODGE_STRENGTH;
        }

        Vector3 target = transform.position + (goalDir + dodge).normalized * DRIBBLE_STEP_DISTANCE;
        SetMoveTarget(target);
        MoveTowards(currentMoveTarget);
    }

    #endregion

    #region ACTIONS - SUPPORT / MARK / DEFEND

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

    private void StopAndResetRotation()
    {
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

        Quaternion defaultRot = character.FormationCoord.DefaultRotation;
        character.Model.rotation = Quaternion.Slerp(
            character.Model.rotation,
            defaultRot,
            ROTATION_SPEED * Time.fixedDeltaTime
        );
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

        float lateralBias = character.FormationCoord.DefaultPosition.x;

        Vector3 supportTarget = ballHolder.transform.position + toGoal * forwardOffset;
        float desiredX = Mathf.Lerp(ballHolder.transform.position.x + lateralBias, character.FormationCoord.DefaultPosition.x, SUPPORT_DESIREDX_LERP);

        supportTarget.x = Mathf.Clamp(desiredX, -maxSideOffset, maxSideOffset);

        foreach (Character mate in teammates)
        {
            if (mate == character || !mate.CanMove()) continue;
            float dist = Vector3.Distance(character.transform.position, mate.transform.position);
            if (dist < MIN_SPACING_BETWEEN_MATES)
            {
                Vector3 sepDir = (character.transform.position - mate.transform.position).normalized;
                supportTarget += sepDir * (MIN_SPACING_BETWEEN_MATES - dist) * SEPARATION_ADJUST_FACTOR;
            }
        }

        if (character.FormationCoord.Position == Position.DF)
            supportTarget = ClampDefensiveLine(supportTarget);

        if (Vector3.Distance(cachedSupportTarget, supportTarget) > CACHE_UPDATE_THRESHOLD)
            cachedSupportTarget = supportTarget;
        else
            supportTarget = Vector3.Lerp(cachedSupportTarget, supportTarget, CACHE_LERP_FACTOR);

        MoveTowards(supportTarget);
    }

    private void ActMark()
    {
        // Marking should never commit/tackle-rush
        ResetCommit();

        // If this role shouldn't mark, just recover shape.
        if (!CanMark())
        {
            SetMoveTarget(character.FormationCoord.DefaultPosition);
            MoveTowards(currentMoveTarget);
            return;
        }

        // If there's no opponent ball-holder, fall back to default position.
        // (You can swap DefaultPosition for GetDefensiveZonePosition() if you want ball-side shifting even in neutral.)
        if (!OpponentHasBall())
        {
            SetMoveTarget(character.FormationCoord.DefaultPosition);
            MoveTowards(currentMoveTarget);
            return;
        }

        // Pick a mark target (assignment system first; fallback to nearest locked)
        Character target = GetAssignedMarkTarget();
        if (!target)
            target = GetLockedMarkTarget();

        // If still none, recover.
        if (!target)
        {
            SetMoveTarget(character.FormationCoord.DefaultPosition);
            MoveTowards(currentMoveTarget);
            return;
        }

        // Defensive shape anchor (keeps you from getting dragged too far)
        Vector3 zonePos = GetDefensiveZonePosition();
        if (character.FormationCoord.Position == Position.DF)
            zonePos = ClampDefensiveLine(zonePos);

        // --- Soft press logic for FW without ball ---
        // Press them like ActDefend (jockey position), but WITHOUT committing.
        bool targetIsForward = target.FormationCoord.Position == Position.FW;
        bool targetHasBall = (PossessionManager.Instance.CurrentCharacter == target);

        Vector3 finalTargetPos;

        if (targetIsForward && !targetHasBall)
        {
            float distToTarget = Vector3.Distance(character.transform.position, target.transform.position);

            // If close enough, jockey (goal-side) like ActDefend, but no commit tracking
            if (distToTarget <= PRESSURE_DISTANCE)
            {
                Vector3 pressDir = (target.transform.position - ownGoal.transform.position).normalized;
                Vector3 pressTarget = target.transform.position - pressDir * PRESS_BACKOFF_DISTANCE;

                // Keep some shape: don't get pulled wildly away from your zone
                finalTargetPos = Vector3.Lerp(zonePos, pressTarget, 0.7f);
            }
            else
            {
                SetMoveTarget(character.FormationCoord.DefaultPosition);
                MoveTowards(currentMoveTarget);
                return;
            }
        }
        else
        {
            SetMoveTarget(character.FormationCoord.DefaultPosition);
            MoveTowards(currentMoveTarget);
            return;
        }

        // Clamp defender advance / line discipline
        finalTargetPos = ClampDefenderAdvance(finalTargetPos);
        if (character.FormationCoord.Position == Position.DF)
            finalTargetPos = ClampDefensiveLine(finalTargetPos);

        SetMoveTarget(finalTargetPos);
        MoveTowards(currentMoveTarget);
    }

    private bool CanMark()
    {
        return character.FormationCoord.Position == Position.DF ||
               character.FormationCoord.Position == Position.MF;
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

        bool isPrimaryDefender = CharacterChangeControlManager.Instance.GetClosestTeammateToBall(character, true) == character || Vector3.Distance(character.transform.position, opponent.transform.position) < 1.5f;

        if (isPrimaryDefender)
        {
            float dist = Vector3.Distance(character.transform.position, opponent.transform.position);

            // If already committing → charge directly
            if (character.FormationCoord.Position == Position.DF || isCommitting)
            {
                Vector3 chargeDir = (opponent.transform.position - transform.position).normalized;
                SetMoveTarget(opponent.transform.position);
                MoveTowards(currentMoveTarget);
                return;
            }

            // Normal jockey / press
            Vector3 pressDir = (opponent.transform.position - ownGoal.transform.position).normalized;
            Vector3 pressTarget = opponent.transform.position - pressDir * PRESS_BACKOFF_DISTANCE;

            SetMoveTarget(pressTarget);
            MoveTowards(currentMoveTarget);

            // Track time spent close
            if (dist <= COMMIT_DISTANCE)
            {
                timeInCommitRange += Time.fixedDeltaTime;

                if (timeInCommitRange >= COMMIT_TIME_REQUIRED)
                {
                    isCommitting = true;
                }
            }
            else
            {
                timeInCommitRange = 0f;
            }
        }
        else
        {
            ActMark();
        }

    }

    #endregion

    #region MOVEMENT & ROTATION

    private void MoveTowards(Vector3 target)
    {
        if (!character.CanMove() || character.IsStateLocked)
            return;

        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < MIN_TARGET_DIST_SQR)
        {
            StopAndResetRotation();
            return;
        }

        dir.Normalize();

        float speed = character.GetMovementSpeed();
        Vector3 desiredVelocity = dir * speed;

        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 velocityDelta = desiredVelocity - horizontalVelocity;
        rb.AddForce(velocityDelta * AI_ACCELERATION, ForceMode.Acceleration);

        RotateFromVelocity();
        character.StartMove();
    }

    private void RotateFromVelocity()
    {
        Vector3 flatVelocity = rb.velocity;
        flatVelocity.y = 0f;

        if (flatVelocity.sqrMagnitude < MIN_VELOCITY_SQR)
            return;

        Quaternion targetRot = Quaternion.LookRotation(flatVelocity);
        character.Model.rotation = Quaternion.Slerp(
            character.Model.rotation,
            targetRot,
            ROTATION_SPEED * Time.fixedDeltaTime
        );
    }

    #endregion

    #region PASS / SHOOT

    private void ActPass()
    {
        Character teammate = GetBestPassTeammate();
        if (!teammate || !character.HasBall()) return;

        character.KickBallTo(teammate.transform.position);
        lastPassReceiver = teammate;
        lastPassTime = Time.time;
    }

    public Character GetBestPassTeammate()
    {
        Character best = null;
        float bestScore = float.MinValue;

        foreach (var mate in teammates)
        {
            if (mate == character || !mate.CanMove()) continue;

            float angleScore = Vector3.Dot(
                (mate.transform.position - character.transform.position).normalized,
                (opponentGoal.transform.position - character.transform.position).normalized);

            float oppDistPenalty = -GetNearestOpponentDistanceTo(mate) * OPPONENT_DISTANCE_PENALTY_MULTIPLIER;
            float distToGoal = GoalManager.Instance.GetDistanceToOpponentGoalZ(mate);

            float score = angleScore * ANGLE_SCORE_WEIGHT + (PASS_SCORE_BASE - oppDistPenalty) - distToGoal;

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

        bool isDirect = SHOOT_DIRECT_DEFAULT;
        bool isLongShoot = SHOOT_LONG_DEFAULT;
        DuelManager.Instance.StartShootDuel(character, isDirect, isLongShoot);
    }

    #endregion

    #region DEFENSIVE ASSIGNMENTS & ZONES

    private Character currentMarkTarget;
    private float markLockUntil;

    private Character GetLockedMarkTarget()
    {
        if (currentMarkTarget &&
            Time.time < markLockUntil &&
            currentMarkTarget.CanMove())
            return currentMarkTarget;

        currentMarkTarget = GetNearestOpponentTo(character);
        markLockUntil = Time.time + MARK_LOCK_TIME;

        return currentMarkTarget;
    }

    private static Dictionary<Character, Character> defensiveAssignments = new Dictionary<Character, Character>();

    private Character GetAssignedMarkTarget()
    {
        // Clean invalid assignments
        if (defensiveAssignments.TryGetValue(character, out var assigned))
        {
            if (assigned && assigned.CanMove())
                return assigned;
        }

        Character best = null;
        float bestScore = float.MaxValue;

        foreach (var opp in opponents)
        {
            if (!opp || !opp.CanMove()) continue;

            // Skip ball holder (primary defender handles)
            if (opp == PossessionManager.Instance.CurrentCharacter)
                continue;

            // Prevent multiple defenders picking same opponent
            if (defensiveAssignments.ContainsValue(opp))
                continue;

            float formationDist = (character.FormationCoord.DefaultPosition - opp.transform.position).sqrMagnitude;

            if (formationDist < bestScore)
            {
                bestScore = formationDist;
                best = opp;
            }
        }

        if (!best)
            return null;

        defensiveAssignments[character] = best;
        return best;
    }

    private void ClearDefensiveAssignments()
    {
        defensiveAssignments.Clear();
    }

    private Vector3 GetDefensiveZonePosition()
    {
        Vector3 zone = character.FormationCoord.DefaultPosition;

        // Shift zone toward ball side
        Vector3 ballPos = ball ? ball.transform.position : zone;
        zone.x = Mathf.Lerp(zone.x, ballPos.x, 0.35f);

        return zone;
    }

    private Vector3 ClampDefenderAdvance(Vector3 pos)
    {
        if (character.FormationCoord.Position != Position.DF)
            return pos;

        if (isEnemyAI)
            pos.z = Mathf.Min(pos.z, DEFENDER_MAX_ADVANCE_AWAY);
        else
            pos.z = Mathf.Max(pos.z, DEFENDER_MAX_ADVANCE_HOME);

        return pos;
    }

    private void ResetCommit()
    {
        isCommitting = false;
        timeInCommitRange = 0f;
    }

    #endregion

    #region DUEL AI

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

    public DuelCommand GetCommandByTrait(Trait trait)
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                return GetRegularCommand();
            case AIDifficulty.Normal:
                if (Random.value < 0.4f && character.HasAffordableMoveWithTrait(trait))
                    return DuelCommand.Move;
                return GetRegularCommand();
            case AIDifficulty.Hard:
                return character.HasAffordableMoveWithTrait(trait)
                    ? DuelCommand.Move
                    : GetRegularCommand();
            default:
                return DuelCommand.Melee;
        }
    }

    public DuelCommand GetRegularCommand() =>
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

    public Move GetMoveByCommandAndTrait(DuelCommand command, Trait trait)
    {
        if (command != DuelCommand.Move) return null;
        return (difficulty == AIDifficulty.Normal)
            ? character.GetRandomAffordableMoveByTrait(trait)
            : character.GetStrongestAffordableMoveByTrait(trait);
    }

    #endregion
}

