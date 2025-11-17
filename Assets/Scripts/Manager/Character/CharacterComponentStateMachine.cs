using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentStateMachine : MonoBehaviour
{
    private Character character;

    [SerializeField] private CharacterState currentState;
    private const float baseAnimationSpeed = 1f;
    private const float controlSpeedMultiplier = 0.005f;

    public CharacterState CurrentState => currentState;

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }

    public void SetCharacterState(CharacterState state) => currentState = state;

    public void StartKick()
    {
        currentState = CharacterState.Kick;
        float controlValue = character.GetBattleStat(Stat.Control);
        //animator.speed = baseAnimationSpeed + controlValue * controlSpeedMultiplier;  // Faster animation for high control players
        //animator.SetTrigger("Kick");
        character.StartStateLock(CharacterState.Kick);
    }

    public void StartControl()
    {
        currentState = CharacterState.Control;
        float controlValue = character.GetBattleStat(Stat.Control);
        //animator.speed = baseAnimationSpeed + controlValue * controlSpeedMultiplier;
        //animator.SetTrigger("Control");
        character.StartStateLock(CharacterState.Control);
    }

    public void StartMove()
    {
        currentState = CharacterState.Move;
        //animator.speed = baseAnimationSpeed;
        //animator.SetTrigger("Move");
    }

    // Called by animation event
    public void OnKickAnimationEnd()
    {
        //animator.speed = baseAnimationSpeed;
        character.ReleaseStateLock();
        currentState = CharacterState.Idle;
    }

    // Called by animation event
    public void OnControlAnimationEnd()
    {
        //animator.speed = baseAnimationSpeed;
        character.ReleaseStateLock();
        currentState = CharacterState.Idle;
    }

    public void OnMoveAnimationEnd()
    {
        //animator.speed = baseAnimationSpeed;
        currentState = CharacterState.Idle;
    }

}
