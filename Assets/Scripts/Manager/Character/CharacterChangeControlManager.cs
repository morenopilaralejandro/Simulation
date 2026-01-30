using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Input;

public class CharacterChangeControlManager : MonoBehaviour
{
    public static CharacterChangeControlManager Instance { get; private set; }

    [SerializeField] private Dictionary<TeamSide, Character> controlledCharacter = new ();
    private BattleManager battleManager;
    private InputManager inputManager;
    private Ball ball => BattleManager.Instance.Ball;
    private int teamSize => BattleManager.Instance.CurrentTeamSize;

    public Dictionary<TeamSide, Character> ControlledCharacter => controlledCharacter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        controlledCharacter = new Dictionary<TeamSide, Character>
        {
            { TeamSide.Home, null },
            { TeamSide.Away, null }
        };
    }

    void Start()
    {
        battleManager = BattleManager.Instance;
        inputManager = InputManager.Instance;
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

        if (inputManager.GetDown(CustomAction.Change))
            TryChangeCharacterManual();
    }

    private bool CanChangeCharacter()
    {
        Character character = GetUserControlledCharacter();

        return character != null &&
               !character.HasBall() &&
               !battleManager.IsTimeFrozen;
    }

    private void TryChangeCharacterManual()
    {
        Character current = GetUserControlledCharacter();
        Character target = battleManager.TargetedCharacter[current.TeamSide];

        if (target != null)
            SetControlledCharacter(target, target.TeamSide);
    }

    public void SetControlledCharacter(Character character, TeamSide teamSide)
    {
        this.controlledCharacter[teamSide] = character;
        CharacterEvents.RaiseControlChange(character, character.TeamSide);
        LogManager.Trace($"[CharacterChangeControlManager] {character.TeamSide.ToString()} control assigned to {character.CharacterId}", this);
    }

    private void HandleOnGained(Character character) 
    {
        SetControlledCharacter(character, character.TeamSide);
    }

    public Character GetUserControlledCharacter() => controlledCharacter[battleManager.GetUserSide()];

    public Character GetClosestTeammateToBall(Character character, bool includeSelf)
    {
        Character nearestCharacter = null;
        List<Character> teammates = character.GetTeammates();
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < teamSize; i++)
        {
            Character teammate = teammates[i];

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

    public Character GetTeammateForShootCombo(Character character)
    {
        Vector3 opponentGoalPos =
            GoalManager.Instance.Goals[character.GetOpponentSide()].transform.position;

        List<Character> teammates = character.GetTeammates();
        Character bestCandidate = null;
        float bestScore = Mathf.Infinity;

        float characterToGoalDist = Vector3.Distance(
            character.transform.position,
            opponentGoalPos);

        float characterX = character.transform.position.x;

        foreach (Character teammate in teammates)
        {
            if (teammate == character || !teammate.CanMove())
                continue;

            float teammateX = teammate.transform.position.x;

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

    public Character GetCharacterForShootBlock(Character opponent)
    {
        Vector3 goalPos =
            GoalManager.Instance.Goals[opponent.GetOpponentSide()].transform.position;

        List<Character> defenders = opponent.GetOpponents();
        Character bestBlocker = null;
        float closestDistance = Mathf.Infinity;

        foreach (Character defender in defenders)
        {
            if (!defender.CanMove())
                continue;

            float defenderToGoalDist = Vector3.Distance(
                defender.transform.position,
                goalPos);

            float opponentToGoalDist = Vector3.Distance(
                opponent.transform.position,
                goalPos);

            // Must be between opponent and goal
            if (defenderToGoalDist >= opponentToGoalDist)
                continue;

            float distToOpponent = Vector3.Distance(
                defender.transform.position,
                opponent.transform.position);

            if (distToOpponent < closestDistance)
            {
                closestDistance = distToOpponent;
                bestBlocker = defender;
            }
        }

        return bestBlocker;
    }

    public Character GetCharacterForDeadBallGeneric(Character opponent)
    {
        List<Character> opponents = opponent.GetOpponents();
        Character closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Character character in opponents)
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

    public void TryChangeOnShootCombo(Character character) 
    {
        Character newCharacter = GetTeammateForShootCombo(character);
        if (newCharacter && !newCharacter.IsEnemyAI) 
            SetControlledCharacter(newCharacter, newCharacter.TeamSide);
    }


}
