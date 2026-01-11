using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Input;

public class CharacterTargetManager : MonoBehaviour
{
    public static CharacterTargetManager Instance { get; private set; }

    private int teamSize => BattleManager.Instance.CurrentTeamSize;
    private float angleThreshold = 30f;
    private CharacterTargetIndicator targetIndicator;
    [SerializeField] private Dictionary<TeamSide, Character> targetedCharacter = new ();

    public Dictionary<TeamSide, Character> TargetedCharacter => targetedCharacter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        this.targetedCharacter.Add(TeamSide.Home, null);
        this.targetedCharacter.Add(TeamSide.Away, null);
    }

    private void OnEnable()
    {
        CharacterEvents.OnTargetChange += HandleOnTargetChange;
    }

    private void OnDisable()
    {
        CharacterEvents.OnTargetChange -= HandleOnTargetChange;
    }

    private void HandleOnTargetChange(Character character, TeamSide teamSide)
    {
        this.targetedCharacter[teamSide] = character;
        //LogManager.Trace($"[CharacterTargetManager] {teamSide.ToString()} target assigned to {character?.CharacterId}", this);
    }

    public void RegisterIndicator(CharacterTargetIndicator indicator)
    {
        targetIndicator = indicator;
    }

    public void UnregisterIndicator()
    {
        targetIndicator = null;
    }


    public Character GetClosestTeammateInDirection(Character character, Vector3 direction)
    {
        List<Character> teammates = character.GetTeammates();

        Character bestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < teamSize; i++)
        {
            Character teammate = teammates[i];
            if (teammate == character || !teammate.CanDuel()) continue;  // Skip self

            Vector3 toTeammate = teammate.transform.position - character.transform.position;
            float angle = Vector3.Angle(direction, toTeammate);

            if (angle < angleThreshold)
            {
                float distance = Vector3.Distance(character.transform.position, teammate.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = teammate;
                }
            }
        }

        return bestTarget;
    }

    public Character GetClosestTeammateToPoint(Character character, Vector3 point)
    {
        List<Character> teammates = character.GetTeammates();

        Character bestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < teamSize; i++)
        {
            Character teammate = teammates[i];

            // Skip self and invalid teammates
            if (!teammate.CanDuel())
                continue;

            float distance = Vector3.Distance(teammate.transform.position, point);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = teammate;
            }
        }

        return bestTarget;
    }

    public void ShowFreeAim(Vector3 startPos, Vector3 endPos)
    {
        targetIndicator.ShowFreeAim(startPos, endPos);
    }

    public void Hide()
    {
        targetIndicator.Hide();
    }

}
