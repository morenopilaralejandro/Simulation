using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

public class CharacterComponentController : MonoBehaviour
{
    #region Constants
    private const float MIN_INPUT_SQR_MAGNITUDE = 0.0001f;
    private const float MIN_ROTATION_SQR_VELOCITY = 0.05f;
    #endregion

    #region Serialized Fields
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    private float acceleration = 50f;
    private float rotationSpeed = 12f;

    [Header("Pass Aiming")]
    private float aimRadius = 4f;
    private float passHoldThreshold = 0.2f;
    #endregion

    #region State
    private Character character;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private bool isAimingPass;
    private float passHoldTimer;
    private Vector3 aimedPassPosition;

    private bool useMouseAiming;
    private Character cachedTarget;
    #endregion

    #region Properties
    private bool IsControlledInternal =>
        BattleManager.Instance.ControlledCharacter[
            BattleTeamManager.Instance.GetUserSide()] == character;

    public bool IsControlled => IsControlledInternal;

    private bool CanProcessInput =>
        IsControlledInternal &&
        !character.IsAutoBattleEnabled &&
        BattleManager.Instance.CurrentPhase == BattlePhase.Battle &&
        !BattleManager.Instance.IsTimeFrozen;
    #endregion

    #region Lifecycle
    public void Initialize(CharacterData data, Character character)
    {
        this.character = character;
        useMouseAiming = !InputManager.Instance.IsAndroid;
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += OnAssignCharacter;
        BattleEvents.OnBattlePhaseChanged += OnBattlePhaseChanged;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= OnAssignCharacter;
        BattleEvents.OnBattlePhaseChanged -= OnBattlePhaseChanged;
    }

    private void Update()
    {
        if (IsControlledInternal && !character.IsAutoBattleEnabled) 
        {
            ReadMovementInput();
            UpdateTargeting();
        }

        if (!CanProcessInput) return;

        BufferShootInput();

        if (!character.CanMove() || BattleUIManager.Instance.IsBattleMenuOpen) return;

        HandlePassInput();
        HandleShootInput();
    }

    private void FixedUpdate()
    {
        if (!CanProcessInput || !character.CanMove() || character.IsStateLocked)
            return;

        HandleMovement();
        HandleRotation();
    }
    #endregion

    #region Input
    private void BufferShootInput()
    {
        InputManager.Instance.GetDown(CustomAction.Shoot);
    }

    private void ReadMovementInput()
    {
        moveInput = InputManager.Instance.GetMove();
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
    }
    #endregion

    #region Targeting
    private void UpdateTargeting()
    {
        if (isAimingPass || BattleEffectManager.Instance.IsPlayingMove || DeadBallManager.Instance.IsUserDefense)
            return;

        Character target = moveDirection.sqrMagnitude > MIN_INPUT_SQR_MAGNITUDE
            ? CharacterTargetManager.Instance.GetClosestTeammateInDirection(character, moveDirection)
            : null;

        CharacterEvents.RaiseTargetChange(target, character.TeamSide);
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        float speed = character.GetMovementSpeed();

        Vector3 desiredVelocity = new Vector3(
            moveInput.x * speed,
            0f,
            moveInput.y * speed
        );

        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 velocityDelta = desiredVelocity - horizontalVelocity;
        rb.AddForce(velocityDelta * acceleration, ForceMode.Acceleration);
    }

    private void HandleRotation()
    {
        Vector3 flatVelocity = rb.velocity;
        flatVelocity.y = 0f;

        if (flatVelocity.sqrMagnitude < MIN_ROTATION_SQR_VELOCITY)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
        character.Model.rotation = Quaternion.Slerp(
            character.Model.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }
    #endregion

    #region Pass
    private void HandlePassInput()
    {
        if (!character.HasBall())
            return;

        if (InputManager.Instance.GetDown(CustomAction.Pass))
            BeginPass();

        if (InputManager.Instance.GetHeld(CustomAction.Pass))
            UpdatePassAim();

        if (InputManager.Instance.GetUp(CustomAction.Pass))
            ExecutePass();
    }

    private void BeginPass()
    {
        passHoldTimer = 0f;
        isAimingPass = false;
        cachedTarget = BattleManager.Instance.TargetedCharacter[character.TeamSide];
    }

    private void UpdatePassAim()
    {
        passHoldTimer += Time.deltaTime;
        if (passHoldTimer < passHoldThreshold)
            return;

        isAimingPass = true;
        cachedTarget = null;
        CharacterEvents.RaiseTargetChange(null, character.TeamSide);

        Vector3 direction = GetPassDirection();
        aimedPassPosition = transform.position + direction * aimRadius;

        CharacterTargetManager.Instance.ShowFreeAim(transform.position, aimedPassPosition);
    }

    private Vector3 GetPassDirection()
    {
        if (useMouseAiming)
        {
            Vector3 mouseWorld =
                InputManager.Instance.ConvertToWorldPositionOnGround(
                    InputManager.Instance.GetMouse());

            return (mouseWorld - transform.position).normalized;
        }

        return moveDirection.sqrMagnitude > MIN_INPUT_SQR_MAGNITUDE
            ? moveDirection.normalized
            : character.Model.forward;
    }

    private void ExecutePass()
    {
        CharacterTargetManager.Instance.Hide();

        if (isAimingPass)
            PassToPosition(aimedPassPosition);
        else if (cachedTarget != null)
            PassToTeammate(cachedTarget);

        isAimingPass = false;
    }

    private void PassToTeammate(Character target)
    {
        character.KickBallTo(target.transform.position);
        CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
    }

    private void PassToPosition(Vector3 position)
    {
        Character receiver =
            CharacterTargetManager.Instance.GetClosestTeammateToPoint(character, position);

        character.KickBallTo(position);

        if (receiver != null)
            CharacterChangeControlManager.Instance.SetControlledCharacter(receiver, character.TeamSide);
    }
    #endregion

    #region Shoot
    private void HandleShootInput()
    {
        if (!character.HasBall()) return;

        if (!InputManager.Instance.ConsumeBuffered(CustomAction.Shoot, out bool isDirect))
            return;

        if (!character.CanShoot() || !DuelManager.Instance.IsResolved)
            return;

        DuelManager.Instance.StartShootDuel(character, isDirect, false);
    }
    #endregion

    #region Events
    private void OnAssignCharacter(Character assigned, Team team, FormationCoord coord)
    {
        if (assigned == character &&
            team.TeamSide != BattleTeamManager.Instance.GetUserSide())
        {
            enabled = false;
        }
    }

    private void OnBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase)
    {
        isAimingPass = false;
    }
    #endregion
}
