    using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;
using Simulation.Enums.Duel;

public class CharacterComponentAI : MonoBehaviour
{
    private Character character;

    [SerializeField] private bool isEnemyAI;
    [SerializeField] private bool isAIEnabled = false;  
    [SerializeField] private AIDifficulty difficulty = AIDifficulty.Hard;
    [SerializeField] private AIState currentState = AIState.Idle;

    public bool IsEnemyAI => isEnemyAI;
    public bool IsAIEnabled => isAIEnabled;
    public AIDifficulty AIDifficulty => difficulty;
    public AIState AIState => currentState ;

    private Ball ball;
    private Goal ownGoal;
    private Goal opponentGoal;
    private List<Character> teammates;
    private float attackDistance = 1f;
    private float defendDistance = 0.4f;
    private float closeDistanceOpponent;
    private float closeDistanceBall;
    private float closeDistanceOpponentGoal = 1.5f;
    //private float closeDistanceOwnGoal = 0.8f;
    private bool pendingKickoffPass = false;
    private Character lastPassReceiver;
    private float lastPassTime = -100f;
    private const float minPassReturnDistance = 1.5f;
    private const float passLoopCooldown = 2f;
    private const float rotationSpeed = 12f;
    private float nextDecisionTime = 0f;
    private float minDecisionDelay;
    private float maxDecisionDelay;
    private const float ballTimeAhead = 0.9f;

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;

        InitializeDecisionDelay();
    }

    private void InitializeDistances(Position position)
    {
        switch (position)
        {
            case Position.GK:
                closeDistanceOpponent = 0.5f;
                closeDistanceBall = 0.5f;
                break;
            case Position.DF:
                closeDistanceOpponent = 2f;
                closeDistanceBall = 2f;
                break;
            default:
                closeDistanceOpponent = 4f;
                closeDistanceBall = 4f;
                break;
        }
    }

    private void InitializeDecisionDelay()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                minDecisionDelay = 0.5f;
                maxDecisionDelay = 0.9f;
                break;
            case AIDifficulty.Normal:
                minDecisionDelay = 0.3f;
                maxDecisionDelay = 0.5f;
                break;
            case AIDifficulty.Hard:
                minDecisionDelay = 0.1f;
                maxDecisionDelay = 0.3f;
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

    private void HandleAssignCharacterToTeamBattle(
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.character == character)
        {
            isEnemyAI = team.TeamSide == TeamSide.Away;
            ownGoal = GoalManager.Instance.GetOwnGoal(this.character);
            opponentGoal = GoalManager.Instance.GetOpponentGoal(this.character);
            teammates = character.GetTeammates();
            InitializeDistances(formationCoord.Position);
            isAIEnabled = true;
        }
    }

    private void HandleBallSpawned(Ball ball)
    {
        this.ball = ball;
    }

    public void EnableAI() => isAIEnabled = true;
    public void EnableAI(bool isAIEnabled) => this.isAIEnabled = isAIEnabled;
    public void DisableAI() => isAIEnabled = false;

    #region AI State & Decision Logic
    private void Update()
    {
        if (character.IsControlled || 
            !isAIEnabled || 
            BattleManager.Instance.IsTimeFrozen)
            return;

        // Decision-making (only sometimes)
        if (Time.time >= nextDecisionTime)
        {
            nextDecisionTime = Time.time + Random.Range(minDecisionDelay, maxDecisionDelay);
            UpdateCurrentAIState();
        }

        ExecuteCurrentAIState();
    }

    private void UpdateCurrentAIState()
    {
        /*
        if (!KickoffManager.Instance.IsKickoffReady) 
        { 
            currentState = AIState.Kickoff; 
            return; 
        }
        */

        if (pendingKickoffPass)
        {
            currentState = AIState.KickoffPass; 
            return;
        }

        if (!character.CanMove()) 
        { 
            currentState = AIState.Idle; 
            return; 
        }

        if (character.HasBall())
        {
            if (HasValidPassTarget()) currentState = AIState.Pass;
            else if (character.CanShoot()) currentState = AIState.Shoot;
            else if (character.IsKeeper) currentState = AIState.Keeper;
            else currentState = AIState.Attack;
        }
        else if (character.IsKeeper)
        {
            currentState = AIState.Keeper;
        }
        else if (OpponentHasBall() && IsOpponentInRange())
        {
            currentState = AIState.Defend;
        }
        else if (ball && ball.IsFree() && IsBallInRange())
        {
            currentState = AIState.ChaseBall;
        }
        else
        {
            currentState = (character.FormationCoord.Position == Position.DF) ? AIState.Defend : AIState.Attack;
        }
    }

    private bool IsOpponentInRange()
    {
        var opponent = PossessionManager.Instance.CurrentCharacter;
        return opponent && Vector3.Distance(character.transform.position, opponent.transform.position) < closeDistanceOpponent;
    }

    private bool IsBallInRange()
    {
        return Vector3.Distance(character.transform.position, ball.transform.position) < closeDistanceBall;
    }

    private bool OpponentHasBall()
    {
        Character otherCharacter = PossessionManager.Instance.CurrentCharacter;
        return otherCharacter && !otherCharacter.IsSameTeam(character);
    }

    private bool HasValidPassTarget()
    {
        return character.FormationCoord.Position != Position.FW && GetBestPassTeammate() != null;
    }
    #endregion

    #region AI Actions

    private void ExecuteCurrentAIState()
    {
        switch (currentState)
        {
            case AIState.Idle:                                  break;
            //case AIState.Kickoff:       ActKickoff();         break;
            //case AIState.KickoffPass:   ActKickoffPass();     break;
            case AIState.ChaseBall:     ActChaseBall();           break;
            case AIState.Attack:        ActAttack();            break;
            case AIState.Pass:          ActPass();              break;
            case AIState.Shoot:         ActShoot();             break;
            case AIState.Defend:        ActDefend();            break;
            case AIState.Keeper:        ActKeeper();            break;
        }
    }
    /*
    private void ActKickoff()
    {
        if (!KickoffManager.Instance.IsAiReady) 
        {
            KickoffManager.Instance.SetTeamReady(character.TeamIndex);   
            KickoffManager.Instance.IsAiReady = true;
        }

        if (IsKickoffCharacter())
            needsPostKickoffPass = true;
    }

    private void ActKickoffPass()
    {
        if (!IsKickoffCharacter())
            return;

        var target = GetKickoffPassTarget();
        if (target) 
        {
            needsPostKickoffPass = false;
            BallBehavior.Instance.KickBall(target.transform.position);
        }
    }
    */

    private bool IsKickoffCharacter() => character.HasBall();

    private Character GetKickoffPassTarget()
    {
        Character best = null; 
        float minDist = float.MaxValue;
        foreach (Character teammate in teammates)
        {
            if (teammate == character || 
                character.IsKeeper || 
                !teammate.CanMove()
            ) 
                continue;
            float dist = Vector3.Distance(character.transform.position, teammate.transform.position);
            if (dist < minDist) 
            { 
                minDist = dist; 
                best = teammate; 
            }
        }
        return best;
    }

    private void ActKeeper()
    {
        MoveTowards(ownGoal.transform.position);
        // Additional keeper logic can be added here (intercept, block, patrol, etc)
    }

    private Vector3 PredictBallFuturePosition(float ballTimeAhead)
    {
        return ball.transform.position + ball.GetVelocity() * ballTimeAhead;
    }

    private void ActChaseBall()
    {
        var predictedPosition = PredictBallFuturePosition(ballTimeAhead);
        MoveTowards(predictedPosition);
    }

    private Vector3 GetTacticalPosition()
    {
        Vector3 basePos = character.FormationCoord.DefaultPosition;
        Vector3 toBall = (ball.transform.position - basePos);
        float scale = 0.3f;
        return basePos + toBall * scale;
    }

    private void ActAttack()
    {
        // Move toward opp goal; separate if crowded
    
        if (character.CanShoot()) return;
        
        Vector3 baseTarget = opponentGoal.transform.position;
        Vector3 separation = Vector3.zero;
        int closeTeammates = 0;

        foreach (Character teammate in teammates)
        {
            if (teammate == character || !teammate.CanMove()) continue;

            float dist = Vector3.Distance(character.transform.position, teammate.transform.position);

            if (dist < attackDistance)
            {
                separation += (character.transform.position - teammate.transform.position) / dist;
                closeTeammates++;
            }
        }

        if (closeTeammates > 0)
            separation /= closeTeammates;

        MoveTowards(baseTarget + separation);
    }

    private void ActDefend()
    {
        var target = GetTacticalPosition();

        var opponent = PossessionManager.Instance.CurrentCharacter;
        if (opponent && Vector3.Distance(character.transform.position, opponent.transform.position) <= closeDistanceOpponent)
        {
            Vector3 separation = Vector3.zero; 
            int count = 0;
            foreach (Character teammate in teammates)
            {
                if (teammate == character || !teammate.CanMove()) continue;

                float dist = Vector3.Distance(character.transform.position, teammate.transform.position);

                if (dist < defendDistance)
                {
                    separation += (character.transform.position - teammate.transform.position) / dist;
                    count++;
                }
            }
            if (count > 0) separation /= count;
            target = opponent.transform.position + separation;
        }

        MoveTowards(target);
    }

    private void MoveTowards(Vector3 target)
    {
        if (!character.CanMove())
            return;

        float speed = character.GetMovementSpeed();
        Vector3 direction = (target - transform.position);
        direction.y = 0f; // ignore vertical offset
        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        // Smooth rotation towards target
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Apply movement in the world space
        Vector3 translation = direction * speed * Time.deltaTime;
        transform.Translate(translation, Space.World);

        // Clamp to playable area bounds
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
        float myDist = GoalManager.Instance.GetDistanceToOpponentGoalZ(character);
        Character best = null; 
        float bestDist = myDist;

        foreach (Character teammate in teammates)
        {
            if (teammate == character || 
                teammate.IsKeeper || 
                !teammate.CanMove()
            ) 
                continue;
    
            if (teammate == lastPassReceiver && 
                Time.time - lastPassTime < passLoopCooldown
            ) 
                continue;

            if (
                Vector3.Distance(character.transform.position, teammate.transform.position) < minPassReturnDistance
            )
                continue;

            float mateDist = GoalManager.Instance.GetDistanceToOpponentGoalZ(teammate);
            if (mateDist < bestDist)
            {
                bestDist = mateDist;
                best = teammate;
            }
        }
        return best;
    }

    private void ActShoot()
    {
        bool isDirect = false;
        DuelManager.Instance.StartShootDuel(character, isDirect);
    }
    #endregion


    public void SetAIDifficulty(AIDifficulty difficulty) => this.difficulty = difficulty;

    private void SetAIDifficultyByTeamLevel(Team team)
    {
        if (team.Level >= 0 && team.Level <= 35) 
        {
            difficulty = AIDifficulty.Easy;
        } 
        else if (team.Level > 35 && team.Level <= 65)
        {
            difficulty = AIDifficulty.Normal;
        }
        else if (team.Level > 65 && team.Level <= 99)
        {
            difficulty = AIDifficulty.Hard;
        }
    }

    private void SetAIDifficultyByTeam(Team team)
    {
        switch (team.TeamId) 
        {
            case "T3":
                difficulty = AIDifficulty.Easy;
                break;
            case "T6":
            case "T4":
            case "T5":
                difficulty = AIDifficulty.Normal;
                break;
            case "T2":
                difficulty = AIDifficulty.Hard;
                break;
        }
    }

    #region Duel AI (Public API)
    public DuelCommand GetCommandByCategory(Category category)
    {
        if (DuelManager.Instance.IsKeeperDuel && category == Category.Dribble)
            return GetRegularCommand();

        switch (difficulty)
        {
            case AIDifficulty.Easy:
                return GetRegularCommand();
            case AIDifficulty.Normal:
                if (Random.value < 0.4f && 
                    character.HasAffordableMoveWithCategory(category)
                )
                    return DuelCommand.Move;
                return GetRegularCommand();
            case AIDifficulty.Hard:
                return character.HasAffordableMoveWithCategory(category) ? 
                    DuelCommand.Move : 
                    GetRegularCommand();
            default: return DuelCommand.Melee;
        }
    }

    private DuelCommand GetRegularCommand()
        => character.GetBattleStat(Stat.Body) > character.GetBattleStat(Stat.Control) ? 
            DuelCommand.Melee : 
            DuelCommand.Ranged;

    public Move GetMoveByCommandAndCategory(DuelCommand command, Category category)
    {
        if (command != DuelCommand.Move) return null;
        return (difficulty == AIDifficulty.Normal) ?
            character.GetRandomAffordableMoveByCategory(category) :
            character.GetStrongestAffordableMoveByCategory(category);
    }
    #endregion
}
