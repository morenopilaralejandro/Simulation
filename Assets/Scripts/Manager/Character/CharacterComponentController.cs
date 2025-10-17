using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class CharacterComponentController : MonoBehaviour
{
    private Character character;

    private float rotationSpeed = 10f;
    [SerializeField] private bool isControlled => BattleManager.Instance.ControlledCharacter[BattleTeamManager.Instance.GetUserSide()] == this.character;

    public bool IsControlled => isControlled;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;    
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
    }

    void Update()
    {
        if (!this.isControlled || BattleManager.Instance.IsTimeFrozen) 
            return;

        HandleTarget();
    
        if (!this.character.HasBall() && 
            InputManager.Instance.GetDown(BattleAction.Change)) 
            HandleChange();

        if (!this.character.CanMove()) 
            return;
            
        HandleMovement();

        //block

        if (!this.character.HasBall()) 
            return;

        if (InputManager.Instance.GetDown(BattleAction.Pass)) 
            HandlePass();

        //dribble


    }

    private void HandleAssignCharacterToTeamBattle(
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.character == character && team.TeamSide != BattleTeamManager.Instance.GetUserSide())
        {
            this.enabled = false;
        }
    }

    private void HandleTarget() 
    {
        Vector2 moveInput = InputManager.Instance.GetMove();
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        Character target = 
            CharacterTargetManager.Instance.GetClosestTeammateInDirection(
            this.character, move);
        CharacterEvents.RaiseTargetChange(target);
    }

    private void HandleMovement() 
    {
        Vector2 moveInput = InputManager.Instance.GetMove();
        float speed = this.character.GetMovementSpeed();
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        if (move.sqrMagnitude > 0.01f)
        {
            // Calculate target rotation (look direction)
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

            // Smoothly rotate towards movement direction
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Apply movement
            transform.Translate(move * speed * Time.deltaTime, Space.World);

            LogManager.Trace($"[CharacterComponentController] " +
                $"Character: {character.CharacterId}, " +
                $"Input: {moveInput}, " +
                $"Speed: {speed}, " +
                $"Position: {transform.position}");
        }
    }

    private void HandlePass() 
    {
        Character target = BattleManager.Instance.TargetedCharacter[this.character.TeamSide];
        if(target)
            PassToTeammate(target);
        else 
            PassForward();
    }

    private void PassToTeammate(Character character) 
    {

    }

    private void PassForward() 
    {

    }

    private void HandleChange() 
    {
        Character target = BattleManager.Instance.TargetedCharacter[this.character.TeamSide];
        if (target)
            ChangeToTarget(target);
        else
            ChangeToClosestToBall();
    }

    private void ChangeToTarget(Character character) 
    {
        CharacterEvents.RaiseControlChange(character);
    }

    private void ChangeToClosestToBall() 
    {
        Character newCharacter = 
            CharacterChangeControlManager.Instance.GetClosestTeammateToBall(
                this.character, includeSelf: false);
        CharacterEvents.RaiseControlChange(newCharacter);
    }

}
