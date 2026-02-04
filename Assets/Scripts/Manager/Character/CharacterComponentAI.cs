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
    private const float TARGET_LOCK_TIME = 0.4f;
    private const float DEFENSIVE_TARGET_LOCK_TIME = 0.1f;
    private const float MARKING_TARGET_LOCK_TIME = 1.4f;

    // Ball & timing
    private const float BALL_TIME_AHEAD = 0.9f;

    // Passing logic
    private const float MIN_PASS_DISTANCE = 1f;
    private const float MAX_PASS_DISTANCE = 3f;
    private const float REQUIRED_FORWARD_GAIN = 1.5f;
    private const float PASS_BLOCK_ANGLE_THRESHOLD = 0.8f;
    private const float PASS_BLOCK_DISTANCE = 0.4f;
    private const float PASS_BLOCK_DISTANCE_SQR = PASS_BLOCK_DISTANCE * PASS_BLOCK_DISTANCE;
    private const float PASS_LOOP_COOLDOWN = 2f;

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
    private const float KEEPER_INTERCEPT_RANGE = 0.75f;
    private const float KEEPER_STANDING_OFFSET = 0.5f;
    private const float KEEPER_PREDICT_AHEAD = 0.3f;

    // Defensive line limits
    private const float HOME_DEFENSIVE_MIN_Z = -7.0f;
    private const float HOME_DEFENSIVE_MAX_Z = -4.0f;
    private const float AWAY_DEFENSIVE_MIN_Z =  4.0f;
    private const float AWAY_DEFENSIVE_MAX_Z =  7.0f;

    // AI traits defaults
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
    private const float SUPPORT_FORWARD_OFFSET_FW_MIN = 1.5f;
    private const float SUPPORT_FORWARD_OFFSET_FW_MAX = 3f;

    // Misc small values
    private const float OPENNESS_DISTANCE_EPS = 0.1f;
    private const float FORWARD_SCORE_WEIGHT = 1.5f;
    private const float ANGLE_SCORE_WEIGHT = 2f;
    private const float OPPONENT_DISTANCE_PENALTY_MULTIPLIER = 0.5f;
    private const float PASS_SCORE_BASE = 10f;

    // Ball prediction tuning
    private const float BALL_PREDICT_DISTANCE_SCALE = 3f;

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
    private const float MARK_LOCK_TIME = 1.2f;
    private const float PRESS_BACKOFF_DISTANCE = 0.8f;
    private const float PRESSURE_DISTANCE = 1f;
    private const float DEFENDER_MAX_ADVANCE_HOME = -6f;
    private const float DEFENDER_MAX_ADVANCE_AWAY = 6f;

    // Commit / tackle logic
    private float timeInCommitRange = 0f;
    private bool isCommitting = false;

    private const float COMMIT_DISTANCE = 1f;
    private const float COMMIT_TIME_REQUIRED = 0.25f;

    // Movement physics
    private const float AI_ACCELERATION = 50f;
    private const float MIN_VELOCITY_SQR = 0.01f;
    private const float MIN_TARGET_DIST_SQR = 0.01f;

    // Cached / initial seed values
    private const float INIT_LAST_PASS_TIME = -100f;

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

    #region FIELDS (CACHED FOR PERFORMANCE)

    private Character character;

    private Ball ball;
    private Transform ballTf;
    private Goal ownGoal;
    private Transform ownGoalTf;
    private Goal opponentGoal;
    private Transform opponentGoalTf;
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

    // Cached transforms to reduce property calls and temporary allocations
    private Transform _transform;
    private Transform modelTf;

    #endregion

    #region INITIALIZATION

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
        this.difficulty = AIDifficulty.Hard;

        _transform = character.transform;
        modelTf = character.Model;
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
        float timeNow = Time.time;
        if (timeNow < targetLockUntil)
            return;

        float lockTime = TARGET_LOCK_TIME;
        switch (currentState)
        {
            case AIState.Defend:
                if (character.FormationCoord.Position == Position.DF)
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
        targetLockUntil = timeNow + lockTime;
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
            ownGoalTf = ownGoal ? ownGoal.transform : null;
            opponentGoalTf = opponentGoal ? opponentGoal.transform : null;
            teammates = character.GetTeammates();
            opponents = character.GetOpponents();
            InitializeDistances(formationCoord.Position);
            InitializeSupportForwardOffset(formationCoord.Position);
            isAIEnabled = true;
            isAutoBattleEnabled = SettingsManager.Instance.IsAutoBattleEnabled;
        }
    }

    private void HandleBallSpawned(Ball ball)
    {
        this.ball = ball;
        ballTf = ball ? ball.transform : null;
    }

    private void HandleAutoBattleToggled(bool enable) => isAutoBattleEnabled = enable;

    public void EnableAI() => isAIEnabled = true;
    public void EnableAI(bool isAIEnabled) => this.isAIEnabled = isAIEnabled;
    public void DisableAI() => isAIEnabled = false;

    #endregion

    #region UPDATE LOOP

    private void FixedUpdate()
    {
        // reduce repeated property lookups
        if ((character.IsControlled && !character.IsAutoBattleEnabled) ||
            !isAIEnabled ||
            BattleManager.Instance.IsTimeFrozen)
            return;

        float timeNow = Time.time;
        if (timeNow >= nextDecisionTime)
        {
            nextDecisionTime = timeNow + Random.Range(minDecisionDelay, maxDecisionDelay);
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

        if (ball != null && ball.IsTraveling)
        {
            // use sqr magnitude for speed
            Vector3 d = _transform.position - ballTf.position;
            float distSqr = d.sqrMagnitude;
            if (distSqr <= closeDistanceBall * closeDistanceBall)
            {
                currentState = AIState.Combo;
                return;
            }
        }

        if (ball != null && ball.IsFree())
        {
            Vector3 d = _transform.position - ballTf.position;
            float distSqr = d.sqrMagnitude;
            if (distSqr <= closeDistanceBall * closeDistanceBall)
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

    private float GetNearestOpponentDistanceTo(Character c)
    {
        // returns real distance (not squared) because callers may expect it;
        // use squared for comparisons, then sqrt at the end once.
        float bestSqr = float.MaxValue;
        if (opponents == null || opponents.Count == 0) return float.MaxValue;
        Vector3 pos = c.transform.position;

        var ops = opponents;
        for (int i = 0; i < ops.Count; i++)
        {
            Character opponent = ops[i];
            if (opponent == null || !opponent.CanMove()) continue;
            Vector3 d = opponent.transform.position - pos;
            float sq = d.sqrMagnitude;
            if (sq < bestSqr) bestSqr = sq;
        }

        if (bestSqr == float.MaxValue) return float.MaxValue;
        return Mathf.Sqrt(bestSqr);
    }

    private Character GetNearestOpponentTo(Character c)
    {
        Character best = null;
        float bestSqr = float.MaxValue;
        if (opponents == null || opponents.Count == 0) return null;
        Vector3 pos = c.transform.position;

        var ops = opponents;
        for (int i = 0; i < ops.Count; i++)
        {
            Character opponent = ops[i];
            if (opponent == null || !opponent.CanMove()) continue;
            Vector3 d = opponent.transform.position - pos;
            float sq = d.sqrMagnitude;
            if (sq < bestSqr)
            {
                bestSqr = sq;
                best = opponent;
            }
        }
        return best;
    }

    private bool HasOpenTeammate()
    {
        if (teammates == null || teammates.Count == 0) return false;
        Character openTeammate = null;
        float bestScore = float.MinValue;

        Vector3 myPos = _transform.position;
        var mates = teammates;
        var ops = opponents;

        for (int i = 0; i < mates.Count; i++)
        {
            Character mate = mates[i];
            if (mate == null || mate == character || !mate.CanMove() || mate.IsKeeper) continue;

            if (mate == lastPassReceiver && Time.time - lastPassTime < PASS_LOOP_COOLDOWN) continue;

            Vector3 matePos = mate.transform.position;
            Vector3 toMate = matePos - myPos;
            float dist = toMate.magnitude; // used for distance checks and openness; keep magnitude once
            if (dist < MIN_PASS_DISTANCE || dist > MAX_PASS_DISTANCE) continue;

            float myDistToGoal = GoalManager.Instance.GetDistanceToOpponentGoalZ(character);
            float mateDistToGoal = GoalManager.Instance.GetDistanceToOpponentGoalZ(mate);
            float forwardGain = myDistToGoal - mateDistToGoal;
            if (forwardGain < REQUIRED_FORWARD_GAIN) continue;

            bool blocked = false;
            // pre-normalize passDir once
            Vector3 passDir = toMate / dist;

            for (int j = 0; j < ops.Count; j++)
            {
                Character opponent = ops[j];
                if (opponent == null || !opponent.CanMove()) continue;

                Vector3 toOpp = opponent.transform.position - myPos;
                float toOppMag = toOpp.magnitude;
                if (toOppMag <= 0f) continue;
                Vector3 toOppN = toOpp / toOppMag;

                float proj = Vector3.Dot(passDir, toOppN);
                if (proj > PASS_BLOCK_ANGLE_THRESHOLD)
                {
                    // compare squared distance to line using cross.sqrMagnitude
                    Vector3 cross = Vector3.Cross(passDir, toOpp);
                    float distToLineSqr = cross.sqrMagnitude / (passDir.sqrMagnitude); // passDir.sqrMagnitude ==1 but keep safe
                    if (distToLineSqr < PASS_BLOCK_DISTANCE_SQR && toOppMag < dist)
                    {
                        blocked = true;
                        break;
                    }
                }
            }

            if (blocked) continue;

            float forwardScore = forwardGain;
            float opennessScore = 1f / (dist + OPENNESS_DISTANCE_EPS);
            float totalScore = forwardScore * FORWARD_SCORE_WEIGHT + opennessScore;

            if (totalScore > bestScore)
            {
                bestScore = totalScore;
                openTeammate = mate;
            }
        }

        return openTeammate != null;
    }

    private bool OpponentHasBall()
    {
        Character otherCharacter = PossessionManager.Instance.CurrentCharacter;
        return otherCharacter != null && !otherCharacter.IsSameTeam(character);
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
        if (ball == null) return;

        Vector3 d = _transform.position - ballTf.position;
        float distSqr = d.sqrMagnitude;

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

    private Vector3 PredictBallFuturePosition(float timeAhead)
    {
        if (ball == null) return Vector3.zero;
        Vector3 vel = ball.GetVelocity();
        return ballTf.position + vel * timeAhead;
    }

    private void ActChaseBall()
    {
        if (ball == null) return;
        if (!ball.IsFree() && PossessionManager.Instance.CurrentCharacter != null) return;

        Character closestCharacter = CharacterChangeControlManager.Instance.GetClosestTeammateToBall(character, true);
        if (character != closestCharacter) return;

        Vector3 myPos = _transform.position;
        Vector3 ballPos = ballTf.position;
        float dist = (myPos - ballPos).magnitude;
        float factor = Mathf.Clamp01(dist / BALL_PREDICT_DISTANCE_SCALE);
        Vector3 predicted = PredictBallFuturePosition(BALL_TIME_AHEAD * factor);
        MoveTowards(predicted);
    }

    private void ActCombo() => ActChaseBall();

    private void ActDribble()
    {
        if (!character.HasBall()) return;

        Vector3 goalDir = (opponentGoalTf.position - _transform.position);
        goalDir.y = 0f;
        goalDir.Normalize();
        Vector3 dodge = Vector3.zero;

        Character opp = GetNearestOpponentTo(character);
        if (opp != null)
        {
            Vector3 diff = opp.transform.position - _transform.position;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < OPPONENT_DODGE_DISTANCE_SQR)
            {
                Vector3 toOpp = (opp.transform.position - _transform.position);
                toOpp.y = 0f;
                toOpp.Normalize();

                // perpendicular to opponent direction, not goal
                Vector3 perp = Vector3.Cross(Vector3.up, toOpp);

                // choose side that moves AWAY from opponent
                if (Vector3.Dot(perp, goalDir) < 0f)
                    perp = -perp;

                dodge = perp * DRIBBLE_DODGE_STRENGTH;
            }
        }

        Vector3 sum = goalDir + dodge;
        sum.Normalize();
        Vector3 target = _transform.position + sum * DRIBBLE_STEP_DISTANCE;
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
        Vector3 v = rb.velocity;
        v.x = 0f;
        v.z = 0f;
        rb.velocity = v;

        Quaternion defaultRot = character.FormationCoord.DefaultRotation;
        modelTf.rotation = Quaternion.Slerp(
            modelTf.rotation,
            defaultRot,
            ROTATION_SPEED * Time.fixedDeltaTime
        );
    }

    private void ActSupport()
    {
        if (character.HasBall()) return;

        Character ballHolder = PossessionManager.Instance.CurrentCharacter;
        if (ballHolder == null || !ballHolder.IsSameTeam(character))
        {
            MoveTowards(character.FormationCoord.DefaultPosition);
            return;
        }

        float maxSideOffset = FIELD_HALF_WIDTH - SIDELINE_BUFFER;

        Vector3 toGoal = (opponentGoalTf.position - ballHolder.transform.position);
        toGoal.y = 0f;
        toGoal.Normalize();
        Vector3 lateral = Vector3.Cross(Vector3.up, toGoal);

        float lateralBias = character.FormationCoord.DefaultPosition.x;

        Vector3 supportTarget = ballHolder.transform.position + toGoal * forwardOffset;
        float desiredX = Mathf.Lerp(ballHolder.transform.position.x + lateralBias, character.FormationCoord.DefaultPosition.x, SUPPORT_DESIREDX_LERP);

        // Clamp desired x
        desiredX = Mathf.Clamp(desiredX, -maxSideOffset, maxSideOffset);
        supportTarget.x = desiredX;

        // separation adjustments (use for loop to avoid enumerator allocations)
        if (teammates != null)
        {
            Vector3 myPos = _transform.position;
            var mates = teammates;
            for (int i = 0; i < mates.Count; i++)
            {
                Character mate = mates[i];
                if (mate == character || !mate.CanMove()) continue;
                Vector3 diff = myPos - mate.transform.position;
                float dist = diff.magnitude;
                if (dist < MIN_SPACING_BETWEEN_MATES && dist > 0f)
                {
                    Vector3 sepDir = diff / dist;
                    supportTarget += sepDir * (MIN_SPACING_BETWEEN_MATES - dist) * SEPARATION_ADJUST_FACTOR;
                }
            }
        }

        if (character.FormationCoord.Position == Position.DF)
            supportTarget = ClampDefensiveLine(supportTarget);

        // smoothing cached support target to reduce jitter allocations
        if ((cachedSupportTarget - supportTarget).sqrMagnitude > CACHE_UPDATE_THRESHOLD * CACHE_UPDATE_THRESHOLD)
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
        if (!OpponentHasBall())
        {
            SetMoveTarget(character.FormationCoord.DefaultPosition);
            MoveTowards(currentMoveTarget);
            return;
        }

        // Pick a mark target (assignment system first; fallback to nearest locked)
        Character target = GetAssignedMarkTarget();
        if (target == null)
            target = GetLockedMarkTarget();

        // If still none, recover.
        if (target == null)
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
        bool targetIsForward = target.FormationCoord.Position == Position.FW;
        bool targetHasBall = (PossessionManager.Instance.CurrentCharacter == target);

        Vector3 finalTargetPos;

        if (targetIsForward)
        {
            float distToTarget = Vector3.Distance(_transform.position, target.transform.position);

            // If close enough, jockey (goal-side) like ActDefend, but no commit tracking
            if (distToTarget <= PRESSURE_DISTANCE)
            {
                Vector3 pressDir = (target.transform.position - ownGoalTf.position);
                pressDir.y = 0f;
                pressDir.Normalize();
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
        bool isOpponentValid = opponent != null && !opponent.IsSameTeam(character);

        if (!isOpponentValid)
        {
            opponent = GetNearestOpponentTo(character);
            if (opponent == null)
            {
                MoveTowards(character.FormationCoord.DefaultPosition);
                return;
            }
        }

        bool isPrimaryDefender = CharacterChangeControlManager.Instance.GetPrimaryDefenderAI(character) == character;

        if (isPrimaryDefender)
        {
            float dist = Vector3.Distance(_transform.position, opponent.transform.position);

            // If already committing → charge directly
            if (character.FormationCoord.Position == Position.DF || isCommitting)
            {
                SetMoveTarget(opponent.transform.position);
                MoveTowards(currentMoveTarget);
                return;
            }

            // Normal jockey / press
            Vector3 pressDir = (opponent.transform.position - ownGoalTf.position);
            pressDir.y = 0f;
            pressDir.Normalize();
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

        Vector3 dir = target - _transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < MIN_TARGET_DIST_SQR)
        {
            StopAndResetRotation();
            return;
        }

        dir.Normalize();

        float speed = character.GetMovementSpeed();
        Vector3 desiredVelocity = dir * speed;

        // use local copy to avoid creating many temporary Vector3s
        Vector3 curVel = rb.velocity;
        Vector3 horizontalVelocity = curVel;
        horizontalVelocity.y = 0f;

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
        modelTf.rotation = Quaternion.Slerp(
            modelTf.rotation,
            targetRot,
            ROTATION_SPEED * Time.fixedDeltaTime
        );
    }

    #endregion

    #region PASS / SHOOT

    private void ActPass()
    {
        Character teammate = GetBestPassTeammate();
        if (teammate == null || !character.HasBall()) return;

        character.KickBallTo(teammate.transform.position);
        lastPassReceiver = teammate;
        lastPassTime = Time.time;
    }

    public Character GetBestPassTeammate()
    {
        if (teammates == null || teammates.Count == 0) return null;

        Character best = null;
        float bestScore = float.MinValue;

        Vector3 myPos = _transform.position;
        Vector3 goalDir = (opponentGoalTf.position - myPos);
        goalDir.y = 0f;
        goalDir.Normalize();

        var mates = teammates;
        for (int i = 0; i < mates.Count; i++)
        {
            Character mate = mates[i];
            if (mate == character || !mate.CanMove()) continue;

            Vector3 matePos = mate.transform.position;
            Vector3 toMate = matePos - myPos;
            float toMateMag = toMate.magnitude;
            if (toMateMag <= 0f) continue;
            if (toMateMag < MIN_PASS_DISTANCE || toMateMag > MAX_PASS_DISTANCE) continue;
            Vector3 toMateN = toMate / toMateMag;

            float angleScore = Vector3.Dot(toMateN, goalDir);

            float oppDist = GetNearestOpponentDistanceTo(mate);
            float oppDistPenalty = -oppDist * OPPONENT_DISTANCE_PENALTY_MULTIPLIER;
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
        if (currentMarkTarget != null &&
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
        // Clean invalid assignments quickly: use TryGetValue to avoid extra lookup
        if (defensiveAssignments.TryGetValue(character, out var assigned))
        {
            if (assigned != null && assigned.CanMove())
                return assigned;
        }

        Character best = null;
        float bestScore = float.MaxValue;

        if (opponents == null) return null;

        var ops = opponents;
        for (int i = 0; i < ops.Count; i++)
        {
            var opp = ops[i];
            if (opp == null || !opp.CanMove()) continue;

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

        if (best == null)
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
        Vector3 ballPos = ball != null ? ballTf.position : zone;
        // lerp only x to reduce computations
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

