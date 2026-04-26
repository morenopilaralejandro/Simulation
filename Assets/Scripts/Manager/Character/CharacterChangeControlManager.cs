using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;

public class CharacterChangeControlManager : MonoBehaviour
{
    public static CharacterChangeControlManager Instance { get; private set; }

    [SerializeField] private Dictionary<TeamSide, CharacterEntityBattle> controlledCharacter = new ();
    private BattleManager battleManager;
    private InputManager inputManager;
    private PossessionManager possessionManager;
    private AudioManager audioManager;
    private Ball ball => BattleManager.Instance.Ball;
    private int teamSize => BattleManager.Instance.CurrentTeamSize;
    private const float MIN_BLOCK_DISTANCE = 0.5f;

    public Dictionary<TeamSide, CharacterEntityBattle> ControlledCharacter => controlledCharacter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        controlledCharacter = new Dictionary<TeamSide, CharacterEntityBattle>
        {
            { TeamSide.Home, null },
            { TeamSide.Away, null }
        };
    }

    void Start()
    {
        battleManager = BattleManager.Instance;
        inputManager = InputManager.Instance;
        possessionManager = PossessionManager.Instance;
        audioManager = AudioManager.Instance;
    }

    private void OnEnable()
    {
        BallEvents.OnGained += HandleOnGained;    
    }

    private void OnDisable()
    {
        BallEvents.OnGained -= HandleOnGained;
    }

    void Update() 
    {
        if (!CanChangeCharacter()) return;

        if (inputManager.GetDown(CustomAction.Battle_ChangeManual))
            TryChangeCharacterManual();

        if(!CanChangeCharacterAuto()) return;

        if (inputManager.GetDown(CustomAction.Battle_ChangeAuto))
            TryChangeCharacterAuto();
    }

    private bool CanChangeCharacter()
    {
        CharacterEntityBattle character = GetUserControlledCharacter();

        return character != null &&
               !character.HasBall() &&
               !battleManager.IsTimeFrozen;
    }

    private bool CanChangeCharacterAuto()
    {
        CharacterEntityBattle controlledCharacter = GetUserControlledCharacter();
        CharacterEntityBattle currentCharacter = possessionManager.CurrentCharacter;
        CharacterEntityBattle lastCharacter = possessionManager.LastCharacter;

        if (currentCharacter != null && !controlledCharacter.IsSameTeam(currentCharacter))
            return true;

        if (currentCharacter == null && lastCharacter != null && !controlledCharacter.IsSameTeam(lastCharacter))
            return true;

        return false;
    }

    private void TryChangeCharacterManual()
    {
        CharacterEntityBattle current = GetUserControlledCharacter();
        CharacterEntityBattle target = battleManager.TargetedCharacter[current.TeamSide];

        if (target == null) return;
        SetControlledCharacter(target, target.TeamSide);
        audioManager.PlaySfx("sfx-ball_change_character");
    }

    private void TryChangeCharacterAuto()
    {
        CharacterEntityBattle current = GetUserControlledCharacter();
        CharacterEntityBattle target = GetTeammateForDefense(current);

        if (target == null) return;
        SetControlledCharacter(target, target.TeamSide);
        audioManager.PlaySfx("sfx-ball_change_character");
    }

    public void SetControlledCharacter(CharacterEntityBattle character, TeamSide teamSide)
    {
        this.controlledCharacter[teamSide] = character;
        CharacterEvents.RaiseControlChange(character, character.TeamSide);
        LogManager.Trace($"[CharacterChangeControlManager] {character.TeamSide.ToString()} control assigned to {character.CharacterId}", this);
    }

    private void HandleOnGained(CharacterEntityBattle character) 
    {
        SetControlledCharacter(character, character.TeamSide);
    }

    public CharacterEntityBattle GetUserControlledCharacter() => controlledCharacter[battleManager.GetUserSide()];

    public CharacterEntityBattle GetClosestTeammateToBall(CharacterEntityBattle character, bool includeSelf)
    {
        CharacterEntityBattle nearestCharacter = null;
        List<CharacterEntityBattle> teammates = character.GetTeammates();
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < teamSize; i++)
        {
            CharacterEntityBattle teammate = teammates[i];

            // Skip self unless includeSelf is true
            if (!includeSelf && teammate == character || !teammate.CanMove())
                continue;
            float dist = Vector3.Distance(
                ball.transform.position, 
                teammate.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestCharacter = teammate;
            }
        }

        return nearestCharacter ?? character;
    }

    public CharacterEntityBattle GetTeammateForShootCombo(CharacterEntityBattle character)
    {
        Vector3 opponentGoalPos =
            GoalManager.Instance.Goals[character.GetOpponentSide()].transform.position;

        List<CharacterEntityBattle> teammates = character.GetTeammates();
        CharacterEntityBattle bestCandidate = null;
        float bestScore = Mathf.Infinity;

        float characterToGoalDist = Vector3.Distance(
            character.transform.position,
            opponentGoalPos);

        float characterX = character.transform.position.x;

        foreach (CharacterEntityBattle teammate in teammates)
        {
            if (teammate == character || !teammate.CanMove())
                continue;

            float teammateToGoalDist = Vector3.Distance(
                teammate.transform.position,
                opponentGoalPos);

            // Must be closer to goal than the passer
            if (teammateToGoalDist >= characterToGoalDist)
                continue;

            // Favor closer to goal and closer to passer
            float score =
                teammateToGoalDist +
                Vector3.Distance(character.transform.position, teammate.transform.position);

            if (score < bestScore)
            {
                bestScore = score;
                bestCandidate = teammate;
            }
        }

        return bestCandidate;
    }

    public CharacterEntityBattle GetCharacterForShootBlock(CharacterEntityBattle opponent)
    {
        // 1. If user-controlled character is ahead of the opponent, do not change
        CharacterEntityBattle userControlled = GetUserControlledCharacter();
        if (userControlled == null) return null;
        if (userControlled.CanMove() && IsCharacterAhead(userControlled, opponent)) return null;

        Vector3 goalPos =
            GoalManager.Instance.Goals[opponent.GetOpponentSide()].transform.position;

        List<CharacterEntityBattle> defenders = opponent.GetOpponents();

        CharacterEntityBattle bestBlocker = null;
        float closestDistance = Mathf.Infinity;

        float opponentToGoalDist = Vector3.Distance(
            opponent.transform.position,
            goalPos);

        foreach (CharacterEntityBattle defender in defenders)
        {
            if (!defender.CanMove())
                continue;

            float defenderToGoalDist = Vector3.Distance(
                defender.transform.position,
                goalPos);

            // Must be between opponent and goal
            if (defenderToGoalDist >= opponentToGoalDist)
                continue;

            float distToOpponent = Vector3.Distance(
                defender.transform.position,
                opponent.transform.position);

            // 2. Enforce minimum distance
            if (distToOpponent < MIN_BLOCK_DISTANCE)
                continue;

            if (distToOpponent < closestDistance)
            {
                closestDistance = distToOpponent;
                bestBlocker = defender;
            }
        }

        return bestBlocker;
    }

    public CharacterEntityBattle GetCharacterForDeadBallGeneric(CharacterEntityBattle opponent)
    {
        List<CharacterEntityBattle> opponents = opponent.GetOpponents();
        CharacterEntityBattle closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (CharacterEntityBattle character in opponents)
        {
            if (!character.CanMove()) continue;

            float dist = Vector3.Distance(
                opponent.transform.position,
                character.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = character;
            }
        }

        return closest;
    }

    public CharacterEntityBattle GetTeammateForDefense(CharacterEntityBattle character)
    {
        CharacterEntityBattle userControlled = GetUserControlledCharacter();

        Vector3 ownGoalPos =
            GoalManager.Instance.Goals[character.TeamSide].transform.position;

        Vector3 ballPos = ball.transform.position;

        List<CharacterEntityBattle> teammates = character.GetTeammates();

        CharacterEntityBattle bestDefender = null;
        float bestScore = Mathf.Infinity;

        const float MIN_BLOCK_DISTANCE = 0.5f;

        float ballToGoalDist = Vector3.Distance(ballPos, ownGoalPos);

        foreach (CharacterEntityBattle teammate in teammates)
        {
            // Ignore current user-controlled character
            if (teammate == userControlled || !teammate.CanMove())
                continue;

            float teammateToGoalDist = Vector3.Distance(
                teammate.transform.position,
                ownGoalPos);

            // Must be between ball and own goal (goal-side)
            if (teammateToGoalDist >= ballToGoalDist)
                continue;

            float distToBall = Vector3.Distance(
                teammate.transform.position,
                ballPos);

            // Enforce minimum block distance
            if (distToBall < MIN_BLOCK_DISTANCE)
                continue;

            /*
             * Advantage score:
             *  - Prefer closer to ball
             *  - Slightly prefer closer to goal to block shooting lanes
             */
            float score =
                distToBall +
                (teammateToGoalDist * 0.5f);

            if (score < bestScore)
            {
                bestScore = score;
                bestDefender = teammate;
            }
        }

        return bestDefender;
    }

    public void TryChangeOnShootCombo(CharacterEntityBattle character) 
    {
        CharacterEntityBattle newCharacter = null;
        if (character.IsEnemyAI)
            newCharacter = GetCharacterForShootBlock(character);
        else 
            newCharacter = GetTeammateForShootCombo(character);

        if(newCharacter == null) return;
        SetControlledCharacter(newCharacter, newCharacter.TeamSide);

    }

    public void TryChangeOnDeadBallGeneric(CharacterEntityBattle opponent) 
    {
        CharacterEntityBattle newCharacter = GetCharacterForDeadBallGeneric(opponent);
        CharacterEntityBattle controlledCharacter = GetUserControlledCharacter();
        if (!opponent.IsEnemyAI || newCharacter == null) return;
        SetControlledCharacter(newCharacter, newCharacter.TeamSide);
    }

    public bool IsCharacterAhead(CharacterEntityBattle character, CharacterEntityBattle other) =>
        character.TeamSide == TeamSide.Home
            ? character.transform.position.z < other.transform.position.z
            : character.transform.position.z > other.transform.position.z;

    public CharacterEntityBattle GetPrimaryDefenderAI(CharacterEntityBattle character)
    {
        Vector3 ownGoalPos =
            GoalManager.Instance.Goals[character.TeamSide].transform.position;

        Vector3 ballPos = ball.transform.position;

        List<CharacterEntityBattle> teammates = character.GetTeammates();

        CharacterEntityBattle bestDefender = null;
        float bestScore = Mathf.Infinity;

        float ballToGoalDist = Vector3.Distance(ballPos, ownGoalPos);

        foreach (CharacterEntityBattle teammate in teammates)
        {
            // Ignore current user-controlled character
            if (!teammate.CanMove() || teammate.IsKeeper)
                continue;

            float teammateToGoalDist = Vector3.Distance(
                teammate.transform.position,
                ownGoalPos);

            float distToBall = Vector3.Distance(
                teammate.transform.position,
                ballPos);

            /*
             * Advantage score:
             *  - Prefer closer to ball
             *  - Slightly prefer closer to goal to block shooting lanes
             */
            float score =
                distToBall +
                (teammateToGoalDist * 0.5f);

            if (score < bestScore)
            {
                bestScore = score;
                bestDefender = teammate;
            }
        }

        return bestDefender;
    }

}
