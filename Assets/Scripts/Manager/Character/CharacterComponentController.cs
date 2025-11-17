using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Input;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

public class CharacterComponentController : MonoBehaviour
{
    #region Fields
    private Character character;

    private Vector2 moveInput;
    private Vector3 move;
    private float moveTolerance = 0.01f;
    private float rotationSpeed = 12f;
    float forwardPassDistance = 1f;
    [SerializeField] private bool isControlled => BattleManager.Instance.ControlledCharacter[BattleTeamManager.Instance.GetUserSide()] == this.character;

    public bool IsControlled => isControlled;
    #endregion

    #region Lifecycle
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
        if (!this.isControlled) 
            return;
            
        //buffer shoot
        InputManager.Instance.GetDown(CustomAction.Shoot);
            
        moveInput = InputManager.Instance.GetMove();
        move = new Vector3(moveInput.x, 0f, moveInput.y);

        HandleTarget();   

        if (BattleManager.Instance.IsTimeFrozen) 
            return;

        if (!character.CanMove()) 
            return;
        
        if(!character.IsStateLocked) 
            HandleMovement();

        //block

        if (!character.HasBall()) 
            return;

        //pass

        if (InputManager.Instance.GetDown(CustomAction.Pass)) 
            HandlePass();

        //dribble

        //shoot
        bool wasBuffered;
        if (InputManager.Instance.ConsumeBuffered(CustomAction.Shoot, out wasBuffered) && 
            character.CanShoot() && 
            DuelManager.Instance.IsResolved &&
            !BattleManager.Instance.IsTimeFrozen) 
            HandleShoot(wasBuffered);

    }
    #endregion

    #region Events
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
    #endregion

    #region Target
    private void HandleTarget() 
    {
        Character target = 
            move.sqrMagnitude > moveTolerance ?
                CharacterTargetManager.Instance.GetClosestTeammateInDirection(
                this.character, move) 
                : null;
            CharacterEvents.RaiseTargetChange(target, this.character.TeamSide);
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        if (move.sqrMagnitude > moveTolerance)
        {
            float speed = this.character.GetMovementSpeed();
            // Calculate target rotation (look direction)
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

            // Smoothly rotate towards movement direction
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Apply movement
            Vector3 translation = move * speed * Time.deltaTime;
            transform.Translate(translation, Space.World);
            transform.position = BoundManager.Instance.ClampCharacter(transform.position);

            character.StartMove();

            /*
            LogManager.Trace($"[CharacterComponentController] " +
                $"Character: {character.CharacterId}, " +
                $"Input: {moveInput}, " +
                $"Speed: {speed}, " +
                $"Position: {transform.position}");
            */
        }
    }
    #endregion


    #region Pass
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
        this.character.KickBallTo(character.transform.position);
        CharacterChangeControlManager.Instance.SetControlledCharacter(character, character.TeamSide);
    }

    private void PassForward() 
    {
        //Vector3 forwardDirection = this.character.transform.forward;
        Vector3 forwardDirection = Vector3.forward;
        Vector3 targetPosition = this.character.transform.position + forwardDirection * forwardPassDistance;
        this.character.KickBallTo(targetPosition);
    }
    #endregion

    #region Shoot
    private void HandleShoot(bool isDirect) 
    {
        LogManager.Trace($"[CharacterComponentController] isDirect: {isDirect}");
        DuelManager.Instance.StartShootDuel(character, isDirect);
    }
    #endregion
}
