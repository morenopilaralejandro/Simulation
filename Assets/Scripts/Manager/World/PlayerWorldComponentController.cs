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
    private const float STUCK_THRESHOLD = 0.15f; // seconds with no progress

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
        rb = playerWorldEntity.Rb;

        // Kinematic + interpolation gives the cleanest MovePosition behaviour
        rb.bodyType = RigidbodyType2D.Kinematic; //only for grid
    }

    // ================================================================
    //  UPDATE  — input only
    // ================================================================

    private void Update()
    {
        if (!_enabled) return;
        ReadInput();
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
    //  FREE MOVEMENT (analog)
    // ================================================================

    private void HandleFreeMovement(float dt)
    {
        // For free movement switch to Dynamic so colliders respond normally
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

            // Use newPos for ALL logic — rb.position is stale until next sim step
            Vector2 newPos = Vector2.MoveTowards(
                rb.position, _gridMoveTarget, stepBudget);

            // ---- stuck detection (use newPos, not rb.position) ----
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

            // ---- arrival check (use newPos, not rb.position) ----
            if (Vector2.Distance(newPos, _gridMoveTarget) < 0.005f)
            {
                DistanceTravelledSinceReset += config.gridSize;

                // If input is held, chain directly into the next step
                // WITHOUT ever setting IsMoving = false
                if (MoveInput.sqrMagnitude > 0.01f)
                {
                    // Use the exact grid target as the origin for the next step
                    // so rounding errors never accumulate
                    Vector2 arrivedAt = _gridMoveTarget;

                    if (TryStartGridStepFrom(arrivedAt))
                    {
                        // Spend leftover movement budget on the new step
                        float distUsed = Vector2.Distance(rb.position, arrivedAt);
                        float leftover = stepBudget - distUsed;

                        if (leftover > 0f)
                        {
                            Vector2 continued = Vector2.MoveTowards(
                                arrivedAt, _gridMoveTarget, leftover);
                            rb.MovePosition(continued);
                        }
                        else
                        {
                            rb.MovePosition(arrivedAt);
                        }
                    }
                    else
                    {
                        // Next step was blocked — snap and stop
                        rb.MovePosition(arrivedAt);
                        _isGridMoving = false;
                        IsMoving = false;
                    }
                }
                else
                {
                    // No input — snap to tile and stop
                    rb.MovePosition(_gridMoveTarget);
                    _isGridMoving = false;
                    IsMoving = false;
                }
            }
            else
            {
                rb.MovePosition(newPos);
            }
        }
        else if (MoveInput.sqrMagnitude > 0.01f)
        {
            TryStartGridStepFrom(SnapToGrid(rb.position));
        }
    }   

    // ================================================================
    //  GRID STEP HELPERS
    // ================================================================

    /// <summary>
    /// Attempts to begin a new grid step from the given origin.
    /// Returns true if the step started, false if blocked or no input.
    /// </summary>
    private bool TryStartGridStepFrom(Vector2 fromPosition)
    {
        Vector2 cardinal = GetCardinalDirection(MoveInput);
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
        // snap back to the nearest grid point behind us
        //rb.MovePosition(SnapToGrid(rb.position));
        _isGridMoving = false;
        IsMoving = false;
        _stuckTimer = 0f;
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
            CancelGridStep();
        else
            CancelFreeMovement();
        //UpdateAnimation();

        MoveInput = Vector2.zero;
        _enabled = false;
    }

    public void ResetDistance() => DistanceTravelledSinceReset = 0f;
}
