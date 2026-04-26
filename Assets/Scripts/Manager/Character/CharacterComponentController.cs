using UnityEngine;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Battle;

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
    private CharacterEntityBattle characterEntityBattle;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private bool isAimingPass;
    private float passHoldTimer;
    private Vector3 aimedPassPosition;

    private bool useMouseAiming;
    private CharacterEntityBattle cachedTarget;
    #endregion

    #region Properties
    private bool IsControlledInternal =>
        BattleManager.Instance.ControlledCharacter[
        BattleTeamManager.Instance.GetUserSide()] == characterEntityBattle;

    public bool IsControlled => IsControlledInternal;

    private bool CanProcessInput =>
        IsControlledInternal &&
        !characterEntityBattle.IsAutoBattleEnabled &&
        BattleManager.Instance.CurrentPhase == BattlePhase.Battle &&
        !BattleManager.Instance.IsTimeFrozen;
    #endregion

    #region Lifecycle
    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
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
        if (IsControlledInternal && !characterEntityBattle.IsAutoBattleEnabled) 
        {
            ReadMovementInput();
            UpdateTargeting();
        }

        if (!CanProcessInput) return;

        BufferShootInput();

        if (!characterEntityBattle.CanMove() || BattleUIManager.Instance.IsBattleMenuOpen) return;

        HandlePassInput();
        HandleShootInput();
    }

    private void FixedUpdate()
    {
        if (!CanProcessInput || !characterEntityBattle.CanMove() || characterEntityBattle.IsStateLocked)
            return;

        HandleMovement();
        HandleRotation();
    }
    #endregion

    #region Input
    private void BufferShootInput()
    {
        InputManager.Instance.GetDown(CustomAction.Battle_Shoot);
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
        if (isAimingPass || BattleEffectManager.Instance.IsPlayingMove || DeadBallManager.Instance.PreventTarget())
            return;

        CharacterEntityBattle target = moveDirection.sqrMagnitude > MIN_INPUT_SQR_MAGNITUDE
            ? CharacterTargetManager.Instance.GetClosestTeammateInDirection(characterEntityBattle, moveDirection)
            : null;

        CharacterEvents.RaiseTargetChange(target, characterEntityBattle.TeamSide);
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        float speed = characterEntityBattle.MovementSpeed;

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
        characterEntityBattle.Model.rotation = Quaternion.Slerp(
            characterEntityBattle.Model.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }
    #endregion

    #region Pass
    private void HandlePassInput()
    {
        if (!characterEntityBattle.HasBall())
            return;

        if (InputManager.Instance.GetDown(CustomAction.Battle_Pass))
            BeginPass();

        if (InputManager.Instance.GetHeld(CustomAction.Battle_Pass))
            UpdatePassAim();

        if (InputManager.Instance.GetUp(CustomAction.Battle_Pass))
            ExecutePass();
    }

    private void BeginPass()
    {
        passHoldTimer = 0f;
        isAimingPass = false;
        cachedTarget = BattleManager.Instance.TargetedCharacter[characterEntityBattle.TeamSide];
    }

    private void UpdatePassAim()
    {
        passHoldTimer += Time.deltaTime;
        if (passHoldTimer < passHoldThreshold)
            return;

        isAimingPass = true;
        cachedTarget = null;
        CharacterEvents.RaiseTargetChange(null, characterEntityBattle.TeamSide);

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
            : characterEntityBattle.Model.forward;
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

    private void PassToTeammate(CharacterEntityBattle target)
    {
        characterEntityBattle.KickBallTo(target.transform.position);
        CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
    }

    private void PassToPosition(Vector3 position)
    {
        CharacterEntityBattle receiver =
            CharacterTargetManager.Instance.GetClosestTeammateToPoint(characterEntityBattle, position);

        characterEntityBattle.KickBallTo(position);

        if (receiver != null)
            CharacterChangeControlManager.Instance.SetControlledCharacter(receiver, characterEntityBattle.TeamSide);
    }
    #endregion

    #region Shoot
    private void HandleShootInput()
    {
        if (!characterEntityBattle.HasBall()) return;

        if (!InputManager.Instance.ConsumeBuffered(CustomAction.Battle_Shoot, out bool isDirect))
            return;

        if (!characterEntityBattle.CanShoot() || !DuelManager.Instance.IsResolved)
            return;

        bool isLongShootStart = !GoalManager.Instance.IsInShootDistance(characterEntityBattle);
        DuelManager.Instance.StartShootDuel(characterEntityBattle, isDirect, isLongShootStart);
    }
    #endregion

    #region Events
    private void OnAssignCharacter(CharacterEntityBattle assigned, Team team, FormationCoord coord)
    {
        if (assigned == characterEntityBattle &&
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
