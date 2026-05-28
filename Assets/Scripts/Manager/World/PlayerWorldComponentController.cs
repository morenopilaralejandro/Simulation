using UnityEngine;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.World;
using Aremoreno.Enums.Animation;

public class PlayerWorldComponentController : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;
    private Rigidbody2D rb;

    public bool IsMoving { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public float DistanceTravelledSinceReset { get; private set; }
    public Vector2 CurrentTilePosition { get; private set; }

    // instantly animate when player presses direction,
    // even before tile movement begins.
    public bool WantsToMove =>
        _currentCardinalInput != Vector2.zero;

    public Vector3 CurrentTilePosition3d()
    {
        return new Vector3(
            CurrentTilePosition.x,
            CurrentTilePosition.y,
            transform.position.z
        );
    }

    [Header("Grid Movement")]
    [SerializeField] private float holdToMoveDelay = 0.04f;
    [SerializeField] private float inputDeadZone = 0.1f;

    private Vector2 _lastFacingInput;
    private bool _wasHoldingDirection;
    private bool _lockFacingDuringMove;

    private bool _isGridMoving;
    private Vector2 _gridMoveTarget;

    private Vector2 _currentCardinalInput;
    private float _inputHeldTime;

    // stuck detection
    private Vector2 _lastPosition;
    private float _stuckTimer;

    private const float STUCK_THRESHOLD = 0.15f;

    #region Initialization

    public void Initialize(
        PlayerWorldEntity playerWorldEntity,
        PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;

        rb = playerWorldEntity.Rb;
        rb.bodyType = RigidbodyType2D.Kinematic;

        CurrentTilePosition = SnapToGrid(rb.position);

        UpdateAnimation();
    }

    #endregion

    #region Unity Loop

    public void OnUpdate()
    {
        if (!playerWorldEntity.IsControlEnabled)
            return;

        ReadInput();
        UpdateFacingAndHoldState();

        // Update animation immediately in Update
        // so direction changes feel instant.
        UpdateAnimation();
    }

    public void OnFixedUpdate()
    {
        if (!playerWorldEntity.IsControlEnabled)
            return;

        if (_isGridMoving)
            HandleGridMovement(Time.fixedDeltaTime);
        else
            TryStartHeldGridStep();
    }

    #endregion

    #region Input

    private void ReadInput()
    {
        MoveInput = InputManager.Instance.GetMoveWorld();

        // Your existing inversion logic preserved
        IsRunning = !InputManager.Instance.GetHeld(CustomAction.World_Run);
    }

    private void UpdateFacingAndHoldState()
    {
        Vector2 cardinal =
            GetCardinalDirection(MoveInput, inputDeadZone);

        bool hasDirection = cardinal != Vector2.zero;

        // NEW PRESS
        if (hasDirection && !_wasHoldingDirection)
        {
            _currentCardinalInput = cardinal;
            _lastFacingInput = cardinal;

            if (!_lockFacingDuringMove) playerWorldEntity.SetFacing(cardinal);

            _inputHeldTime = 0f;
        }
        // DIRECTION CHANGED
        else if (hasDirection && cardinal != _lastFacingInput)
        {
            _currentCardinalInput = cardinal;
            _lastFacingInput = cardinal;

            // instantly face new direction
            if (!_lockFacingDuringMove) playerWorldEntity.SetFacing(cardinal);

            // restart hold delay
            _inputHeldTime = 0f;
        }
        // CONTINUE HOLD
        else if (hasDirection)
        {
            _inputHeldTime += Time.deltaTime;
        }
        // RELEASE
        else
        {
            _currentCardinalInput = Vector2.zero;
            _inputHeldTime = 0f;
        }

        _wasHoldingDirection = hasDirection;
    }

    #endregion

    #region Grid Movement

    private void TryStartHeldGridStep()
    {
        if (_currentCardinalInput == Vector2.zero)
            return;

        if (_inputHeldTime < holdToMoveDelay)
            return;

        TryStartGridStepInDirection(
            SnapToGrid(rb.position),
            _currentCardinalInput
        );
    }

    private void HandleGridMovement(float dt)
    {
        float speed =
            IsRunning
                ? config.runSpeed
                : config.walkSpeed;

        float stepBudget = speed * dt;

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            _gridMoveTarget,
            stepBudget
        );

        // stuck detection
        if (Vector2.Distance(newPos, _lastPosition) < 0.001f)
        {
            _stuckTimer += dt;

            if (_stuckTimer >= STUCK_THRESHOLD)
            {
                CancelGridStep();
                return;
            }
        }
        else
        {
            _stuckTimer = 0f;
        }

        _lastPosition = newPos;

        // arrival
        if (Vector2.Distance(newPos, _gridMoveTarget) < 0.005f)
        {
            rb.MovePosition(_gridMoveTarget);

            _isGridMoving = false;
            IsMoving = false;

            _lockFacingDuringMove = false;

            CurrentTilePosition = _gridMoveTarget;

            WorldManager.Instance.OnTileArrived(IsRunning);

            if (!playerWorldEntity.IsControlEnabled)
                return;

            // chain movement
            if (_currentCardinalInput != Vector2.zero &&
                _inputHeldTime >= holdToMoveDelay)
            {
                TryStartGridStepInDirection(
                    _gridMoveTarget,
                    _currentCardinalInput
                );
            }
        }
        else
        {
            rb.MovePosition(newPos);
        }
    }

    private bool TryStartGridStepInDirection(
        Vector2 fromPosition,
        Vector2 cardinal)
    {
        if (cardinal == Vector2.zero)
            return false;

        if (!_lockFacingDuringMove) playerWorldEntity.SetFacing(cardinal);

        Vector2 target =
            SnapToGrid(
                fromPosition +
                cardinal * config.gridSize
            );

        if (IsPathBlocked(
                fromPosition,
                cardinal,
                config.gridSize))
        {
            return false;
        }

        _gridMoveTarget = target;

        _isGridMoving = true;
        IsMoving = true;

        _lockFacingDuringMove = true;

        _stuckTimer = 0f;
        _lastPosition = fromPosition;

        return true;
    }

    private bool IsPathBlocked(
        Vector2 origin,
        Vector2 direction,
        float distance)
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            origin,
            config.collisionCastRadius,
            direction,
            distance,
            config.collisionMask
        );

        return hit.collider != null;
    }

    private void CancelGridStep()
    {
        _isGridMoving = false;
        IsMoving = false;
        _lockFacingDuringMove = false;
        _stuckTimer = 0f;
    }

    #endregion

    #region Animation

    private void UpdateAnimation()
    {
        // animate immediately on directional input,
        // not only after movement begins.

        bool shouldAnimateMove =
            WantsToMove || IsMoving;

        if (shouldAnimateMove)
        {
            if (IsRunning)
            {
                playerWorldEntity.Play(
                    CharacterAnimationState.Run,
                    playerWorldEntity.FacingDirection
                );
            }
            else
            {
                playerWorldEntity.Play(
                    CharacterAnimationState.Walk,
                    playerWorldEntity.FacingDirection
                );
            }
        }
        else
        {
            playerWorldEntity.Play(
                CharacterAnimationState.Idle,
                playerWorldEntity.FacingDirection
            );
        }
    }

    #endregion

    #region Utility

    private static Vector2 GetCardinalDirection(
        Vector2 raw,
        float deadZone)
    {
        if (raw.sqrMagnitude < deadZone * deadZone)
            return Vector2.zero;

        if (Mathf.Abs(raw.x) >= Mathf.Abs(raw.y))
        {
            return new Vector2(
                Mathf.Sign(raw.x),
                0f
            );
        }

        return new Vector2(
            0f,
            Mathf.Sign(raw.y)
        );
    }

    private Vector2 SnapToGrid(Vector2 pos)
    {
        float g = config.gridSize;
        float half = g * 0.5f;

        return new Vector2(
            Mathf.Round((pos.x - half) / g) * g + half,
            Mathf.Round((pos.y - half) / g) * g + half
        );
    }

    #endregion

    #region Public

    public void StopMovement()
    {
        if (_isGridMoving)
        {
            rb.MovePosition(
                SnapToGrid(rb.position)
            );
        }

        _isGridMoving = false;
        IsMoving = false;
        _stuckTimer = 0f;

        _currentCardinalInput = Vector2.zero;
        _inputHeldTime = 0f;

        MoveInput = Vector2.zero;

        CurrentTilePosition =
            SnapToGrid(rb.position);

        playerWorldEntity.SetControlEnabled(false);

        UpdateAnimation();
    }

    public void ResetDistance()
    {
        DistanceTravelledSinceReset = 0f;
    }

    #endregion
}
