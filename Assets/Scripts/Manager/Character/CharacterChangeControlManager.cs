using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Input;

public class CharacterChangeControlManager : MonoBehaviour
{
    public static CharacterChangeControlManager Instance { get; private set; }

    private Ball ball => BattleManager.Instance.Ball;
    private int teamSize => BattleManager.Instance.CurrentTeamSize;
    [SerializeField] private Dictionary<TeamSide, Character> controlledCharacter = new ();

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

        this.controlledCharacter.Add(TeamSide.Home, null);
        this.controlledCharacter.Add(TeamSide.Away, null);
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        CharacterEvents.OnControlChange += HandleOnControlChange;
    }

    private void OnDisable()
    {
        CharacterEvents.OnControlChange += HandleOnControlChange;
    }

    private void HandleOnControlChange(Character character, TeamSide teamSide)
    {
        this.controlledCharacter[teamSide] = character;
        LogManager.Trace($"[CharacterChangeControlManager] {teamSide.ToString()} control assigned to {character?.CharacterId}", this);
    }

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



}
