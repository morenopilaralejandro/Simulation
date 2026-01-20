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
    [SerializeField] private Rigidbody rb;

    private Character character;

    private Vector2 moveInput;
    private Vector3 desiredVelocity;
    private Vector3 move;
    private float moveTolerance = 0.01f;
    private float rotationSpeed = 12f;

    private float acceleration = 50f;

    private bool isAimingPass = false;
    private Vector3 aimedPassPosition;
    private float aimRadius = 4f;
    private float holdThreshold = 0.2f;
    private float passButtonHoldTime = 0f;
    private bool useMouseAiming;
    private Character cachedTarget;

    [SerializeField] private bool isControlled => BattleManager.Instance.ControlledCharacter[BattleTeamManager.Instance.GetUserSide()] == this.character;

    public bool IsControlled => isControlled;
    #endregion

    #region Lifecycle
    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;

        useMouseAiming = !InputManager.Instance.IsAndroid;
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;
        BattleEvents.OnBattlePhaseChanged += HandleBattlePhaseChanged;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
        BattleEvents.OnBattlePhaseChanged += HandleBattlePhaseChanged;
    }

    void Update()
    {
        if (!this.isControlled || character.IsAutoBattleEnabled)
            return;
            
        //buffer shoot
        InputManager.Instance.GetDown(CustomAction.Shoot);
            
        moveInput = InputManager.Instance.GetMove();
        move = new Vector3(moveInput.x, 0f, moveInput.y);

        HandleTarget();   

        if (BattleManager.Instance.IsTimeFrozen) 
            return;

        if (BattleManager.Instance.CurrentPhase != BattlePhase.Battle) 
            return;

        if (!character.CanMove()) 
            return;
        
        //if(!character.IsStateLocked) 
            //HandleMovement();

        if(BattleUIManager.Instance.IsBattleMenuOpen)
            return;

        //block

        if (!character.HasBall()) 
            return;

        //pass
        bool passDown = InputManager.Instance.GetDown(CustomAction.Pass);
        bool passHeld = InputManager.Instance.GetHeld(CustomAction.Pass);
        bool passUp   = InputManager.Instance.GetUp(CustomAction.Pass);

        if (passDown)
            StartPassMode();

        if (passHeld)
            UpdatePassIndicator();

        if (passUp) 
        {
            HandlePass();
            return;
        }
        //dribble

        //shoot
        bool wasBuffered;
        if (InputManager.Instance.ConsumeBuffered(CustomAction.Shoot, out wasBuffered) && 
            character.CanShoot() && 
            DuelManager.Instance.IsResolved &&
            !BattleManager.Instance.IsTimeFrozen) 
            HandleShoot(wasBuffered);

    }

    private void FixedUpdate()
    {
        if (!this.isControlled || character.IsAutoBattleEnabled)
            return;

        if (BattleManager.Instance.IsTimeFrozen) 
            return;

        if (BattleManager.Instance.CurrentPhase != BattlePhase.Battle) 
            return;

        if (!character.CanMove()) 
            return;
        
        if(!character.IsStateLocked) {
        HandleMovement();
        HandleRotation();
}
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

    private void HandleBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase)
    {
        isAimingPass = false;
    }
    #endregion

    #region Target
    private void HandleTarget() 
    {
        if(isAimingPass || BattleEffectManager.Instance.IsPlayingMove) return;

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
        float moveSpeed = character.GetMovementSpeed();
        desiredVelocity.Set(
            moveInput.x * moveSpeed,
            rb.velocity.y,
            moveInput.y * moveSpeed
        );

        rb.velocity = Vector3.MoveTowards(
            rb.velocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime
        );
    }

    private void HandleRotation()
    {
        Vector3 flatVelocity = rb.velocity;
        flatVelocity.y = 0f;

        if (flatVelocity.sqrMagnitude > 0.05f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
            character.Model.rotation = Quaternion.Slerp(
                character.Model.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
    #endregion

    #region Pass
    private void StartPassMode()
    {
        passButtonHoldTime = 0f;
        isAimingPass = false;
        cachedTarget = BattleManager.Instance.TargetedCharacter[this.character.TeamSide];
    }

    private void UpdatePassIndicator()
    {
        passButtonHoldTime += Time.deltaTime;

        if (passButtonHoldTime <= holdThreshold) return;

        cachedTarget = null;
        CharacterEvents.RaiseTargetChange(null, this.character.TeamSide);
        isAimingPass = true;

        Vector3 center = transform.position;
        Vector3 direction;
        if (useMouseAiming)
        {
            Vector3 mouseWorld = InputManager.Instance.ConvertToWorldPositionOnGround(InputManager.Instance.GetMouse());
            direction = (mouseWorld - transform.position).normalized;
        }
        else
        {
            direction = (moveInput.sqrMagnitude > moveTolerance)
                ? new Vector3(moveInput.x, 0, moveInput.y).normalized
                : character.Model.transform.forward;
        }  

        aimedPassPosition = center + direction * aimRadius;
        CharacterTargetManager.Instance.ShowFreeAim(center, aimedPassPosition);
    }

    private void HandlePass() 
    {
        CharacterTargetManager.Instance.Hide();

        if(isAimingPass)
            PassToPosition(aimedPassPosition);

        if(cachedTarget)
            PassToTeammate(cachedTarget);
      
        isAimingPass = false;
    }

    private void PassToTeammate(Character character) 
    {
        this.character.KickBallTo(character.transform.position);
        CharacterChangeControlManager.Instance.SetControlledCharacter(character, character.TeamSide);
    }

    private void PassToPosition(Vector3 position) 
    {
        Character newCharacter = CharacterTargetManager.Instance.GetClosestTeammateToPoint(this.character, position);
        this.character.KickBallTo(position);
        if (newCharacter != null) 
            CharacterChangeControlManager.Instance.SetControlledCharacter(newCharacter, character.TeamSide);
    }
    #endregion

    #region Shoot
    private void HandleShoot(bool isDirect) 
    {
        LogManager.Trace($"[CharacterComponentController] isDirect: {isDirect}");
        DuelManager.Instance.StartShootDuel(character, isDirect, false);
    }
    #endregion
}
