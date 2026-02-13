using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentStateMachine : MonoBehaviour
{
    private Character character;
    private CharacterEntityBattle characterEntityBattle;

    [SerializeField] private CharacterState currentState;
    private const float baseAnimationSpeed = 1f;
    private const float controlSpeedMultiplier = 0.005f;

    public CharacterState CurrentState => currentState;

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.character = characterEntityBattle.Character;
        this.characterEntityBattle = characterEntityBattle;
    }

    public void SetCharacterState(CharacterState state) => currentState = state;

    public void StartKick()
    {
        currentState = CharacterState.Kick;
        float controlValue = characterEntityBattle.GetBattleStat(Stat.Control);
        //animator.speed = baseAnimationSpeed + controlValue * controlSpeedMultiplier;  // Faster animation for high control players
        //animator.SetTrigger("Kick");
        characterEntityBattle.StartStateLock(CharacterState.Kick);
    }

    public void StartControl()
    {
        currentState = CharacterState.Control;
        float controlValue = characterEntityBattle.GetBattleStat(Stat.Control);
        //animator.speed = baseAnimationSpeed + controlValue * controlSpeedMultiplier;
        //animator.SetTrigger("Control");
        characterEntityBattle.StartStateLock(CharacterState.Control);
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
        characterEntityBattle.ReleaseStateLock();
        currentState = CharacterState.Idle;
    }

    // Called by animation event
    public void OnControlAnimationEnd()
    {
        //animator.speed = baseAnimationSpeed;
        characterEntityBattle.ReleaseStateLock();
        currentState = CharacterState.Idle;
    }

    public void OnMoveAnimationEnd()
    {
        //animator.speed = baseAnimationSpeed;
        currentState = CharacterState.Idle;
    }

}
