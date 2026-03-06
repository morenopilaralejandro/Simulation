using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.World;

public class PlayerWorldComponentController : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;
    private Rigidbody2D rb;

    public bool IsMoving { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public float DistanceTravelledSinceReset { get; private set; }
    public bool IsEnabled => _enabled;

    private bool _enabled = false;
    private Vector3 _velocity;
    private bool _isGridMoving;
    private Vector2 _gridMoveTarget;
    private float acceleration = 50f;

    // stuck detection
    private Vector2 _lastPosition;
    private float _stuckTimer;
    private const float STUCK_THRESHOLD = 0.15f;

    // ---- INPUT BUFFER ----
    private Vector2 _bufferedDirection;       // the cardinal direction that was buffered
    private float _bufferTimer;               // time remaining on the current buffer
    private const float INPUT_BUFFER_WINDOW = 0.1f; // how long (seconds) a buffered input stays valid

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
        rb = playerWorldEntity.Rb;

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // ================================================================
    //  UPDATE  — input only
    // ================================================================

    private void Update()
    {
        if (!_enabled) return;
        ReadInput();
        UpdateInputBuffer();
        UpdateAnimation();
    }

    // ================================================================
    //  FIXED UPDATE  — physics movement
    // ================================================================

    private void FixedUpdate()
    {
        if (!_enabled) return;

        float dt = Time.fixedDeltaTime;

        if (config.gridBasedMovement)
            HandleGridMovement(dt);
        else
            HandleFreeMovement(dt);
    }

    // ================================================================
    //  INPUT
    // ================================================================

    private void ReadInput()
    {
        MoveInput = InputManager.Instance.GetMove();
        IsRunning = !InputManager.Instance.GetHeld(CustomAction.Battle_Pass);
    }

    // ================================================================
    //  INPUT BUFFER
    // ================================================================

    /// <summary>
    /// Called every Update. If the player is currently mid-step and presses
    /// a direction, we store it so it can be consumed the instant the
    /// current step finishes.
    /// </summary>
    private void UpdateInputBuffer()
    {
        if (!config.gridBasedMovement) return;

        // Tick down the buffer timer
        if (_bufferTimer > 0f)
            _bufferTimer -= Time.deltaTime;

        // If there's directional input right now, refresh the buffer
        Vector2 cardinal = GetCardinalDirection(MoveInput);
        if (cardinal != Vector2.zero)
        {
            _bufferedDirection = cardinal;
            _bufferTimer = INPUT_BUFFER_WINDOW;
        }
    }

    /// <summary>
    /// Tries to consume the buffered input. Returns the buffered cardinal
    /// direction and clears the buffer, or Vector2.zero if nothing is buffered.
    /// </summary>
    private Vector2 ConsumeBuffer()
    {
        if (_bufferTimer > 0f && _bufferedDirection != Vector2.zero)
        {
            Vector2 dir = _bufferedDirection;
            ClearBuffer();
            return dir;
        }
        return Vector2.zero;
    }

    private void ClearBuffer()
    {
        _bufferedDirection = Vector2.zero;
        _bufferTimer = 0f;
    }

    // ================================================================
    //  FREE MOVEMENT (analog)
    // ================================================================

    private void HandleFreeMovement(float dt)
    {
        float speed = IsRunning ? config.runSpeed : config.walkSpeed;

        Vector2 desiredVelocity = MoveInput * speed;
        Vector2 velocityDelta = desiredVelocity - rb.velocity;
        rb.AddForce(velocityDelta * acceleration, ForceMode2D.Force);

        IsMoving = rb.velocity.sqrMagnitude > 0.01f;
    }

    // ================================================================
    //  GRID-BASED MOVEMENT
    // ================================================================

    private void HandleGridMovement(float dt)
    {
        if (_isGridMoving)
        {
            float speed = IsRunning ? config.runSpeed : config.walkSpeed;
            float stepBudget = speed * dt;

            Vector2 newPos = Vector2.MoveTowards(
                rb.position, _gridMoveTarget, stepBudget);

            // ---- stuck detection ----
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

            WorldManagerEncounter.Instance.Tick(_isGridMoving, speed, dt);

            // Tick() may have triggered an encounter and called StopMovement()
            if (!_isGridMoving || !_enabled) return;

            // ---- arrival check ----
            if (Vector2.Distance(newPos, _gridMoveTarget) < 0.005f)
            {
                // Snap precisely to the target
                rb.MovePosition(_gridMoveTarget);
                _isGridMoving = false;
                IsMoving = false;

                // === INPUT BUFFER: try to chain immediately ===
                // 1) Try the buffered direction first (was pressed during the step)
                Vector2 buffered = ConsumeBuffer();
                if (buffered != Vector2.zero)
                {
                    if (TryStartGridStepInDirection(_gridMoveTarget, buffered))
                        return;
                }

                // 2) If buffer didn't produce a step, try current live input
                if (MoveInput.sqrMagnitude > 0.01f)
                {
                    TryStartGridStepFrom(_gridMoveTarget);
                }
            }
            else
            {
                rb.MovePosition(newPos);
            }
        }
        else if (MoveInput.sqrMagnitude > 0.01f)
        {
            // Not moving — start a fresh step from current position
            TryStartGridStepFrom(SnapToGrid(rb.position));
        }
    }

    // ================================================================
    //  GRID STEP HELPERS
    // ================================================================

    /// <summary>
    /// Attempts to begin a new grid step from the given origin using the
    /// current MoveInput to determine direction.
    /// Returns true if the step started, false if blocked or no input.
    /// </summary>
    private bool TryStartGridStepFrom(Vector2 fromPosition)
    {
        Vector2 cardinal = GetCardinalDirection(MoveInput);
        return TryStartGridStepInDirection(fromPosition, cardinal);
    }

    /// <summary>
    /// Attempts to begin a new grid step from the given origin in an
    /// explicit cardinal direction.
    /// Returns true if the step started, false if blocked or direction is zero.
    /// </summary>
    private bool TryStartGridStepInDirection(Vector2 fromPosition, Vector2 cardinal)
    {
        if (cardinal == Vector2.zero) return false;

        playerWorldEntity.SetFacing(cardinal);

        Vector2 target = SnapToGrid(fromPosition + cardinal * config.gridSize);

        if (IsPathBlocked(fromPosition, cardinal, config.gridSize))
            return false;

        _gridMoveTarget = target;
        _isGridMoving = true;
        IsMoving = true;
        _stuckTimer = 0f;
        _lastPosition = fromPosition;
        return true;
    }

    /// <summary>
    /// CircleCast along the step direction. Returns true if something
    /// on collisionMask is in the way.
    /// </summary>
    private bool IsPathBlocked(Vector2 origin, Vector2 direction, float distance)
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
        _stuckTimer = 0f;
        ClearBuffer();
    }

    private void CancelFreeMovement()
    {
        IsMoving = false;
        _velocity = Vector3.zero;
        _isGridMoving = false;
        _stuckTimer = 0f;
    }

    private static Vector2 GetCardinalDirection(Vector2 raw)
    {
        if (raw.sqrMagnitude < 0.01f) return Vector2.zero;

        if (Mathf.Abs(raw.x) >= Mathf.Abs(raw.y))
            return new Vector2(Mathf.Sign(raw.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(raw.y));
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

    // ================================================================
    //  ANIMATION
    // ================================================================

    private void UpdateAnimation()
    {
        // ...
    }

    // ================================================================
    //  PUBLIC API
    // ================================================================

    public void SetControlEnabled(bool enabled) => _enabled = enabled;

    public void StopMovement()
    {
        if (config.gridBasedMovement)
        {
            if (_isGridMoving)
            {
                rb.MovePosition(SnapToGrid(rb.position));
            }
            CancelGridStep();
        }
        else
        {
            CancelFreeMovement();
        }

        MoveInput = Vector2.zero;
        ClearBuffer();
        _enabled = false;
    }

    public void ResetDistance() => DistanceTravelledSinceReset = 0f;
}
