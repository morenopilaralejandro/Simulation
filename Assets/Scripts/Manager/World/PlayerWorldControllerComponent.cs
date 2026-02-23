// PlayerMovementController.cs
using UnityEngine;
using Simulation.Enums.World;

[RequireComponent(typeof(CharacterController))]
public class PlayerWorldControllerComponent : MonoBehaviour
{
    [Header("Runtime — set by manager")]
    [SerializeField] private PlayerWorldConfig config;

    public FacingDirection CurrentFacing { get; private set; } = FacingDirection.Down;
    public bool IsMoving { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public float DistanceTravelledSinceReset { get; private set; }

    private CharacterController _cc;
    private bool _enabled = true;
    private Vector3 _velocity;

    // Grid-based movement fields
    private bool _isGridMoving;
    private Vector3 _gridMoveTarget;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public void Initialize(PlayerWorldConfig cfg)
    {
        config = cfg;
    }

    // Called every frame by PlayerWorldManager
    public void Tick(float deltaTime)
    {
        if (!_enabled) return;

        ReadInput();

        if (config.gridBasedMovement)
            HandleGridMovement(deltaTime);
        else
            HandleFreeMovement(deltaTime);

        UpdateAnimation();
    }

    // ================================================================
    //  INPUT
    // ================================================================

    private void ReadInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (!config.allowDiagonalMovement)
        {
            // Prioritise horizontal if both pressed
            if (Mathf.Abs(h) > 0.01f && Mathf.Abs(v) > 0.01f)
                v = 0f;
        }

        MoveInput = new Vector2(h, v);
        IsRunning = Input.GetKey(KeyCode.LeftShift);
    }

    // ================================================================
    //  FREE MOVEMENT (analog)
    // ================================================================

    private void HandleFreeMovement(float dt)
    {
        Vector3 direction = new Vector3(MoveInput.x, 0f, MoveInput.y).normalized;
        float speed = IsRunning ? config.runSpeed : config.walkSpeed;

        IsMoving = direction.sqrMagnitude > 0.01f;

        if (IsMoving)
        {
            UpdateFacing(MoveInput);

            _velocity = direction * speed;

            // Apply gravity
            if (!_cc.isGrounded)
                _velocity.y -= 9.81f * dt;

            _cc.Move(_velocity * dt);
            DistanceTravelledSinceReset += speed * dt;
        }
        else
        {
            // Still apply gravity
            if (!_cc.isGrounded)
            {
                _velocity.y -= 9.81f * dt;
                _cc.Move(_velocity * dt);
            }
        }
    }

    // ================================================================
    //  GRID-BASED MOVEMENT
    // ================================================================

    private void HandleGridMovement(float dt)
    {
        if (_isGridMoving)
        {
            float speed = IsRunning ? config.runSpeed : config.walkSpeed;
            Vector3 newPos = Vector3.MoveTowards(transform.position,
                                                  _gridMoveTarget, speed * dt);
            _cc.Move(newPos - transform.position);

            if (Vector3.Distance(transform.position, _gridMoveTarget) < 0.01f)
            {
                _isGridMoving = false;
                IsMoving = false;
                DistanceTravelledSinceReset += config.gridSize;
            }
        }
        else if (MoveInput.sqrMagnitude > 0.01f)
        {
            UpdateFacing(MoveInput);

            Vector3 dir = new Vector3(
                Mathf.Round(MoveInput.x),
                0f,
                Mathf.Round(MoveInput.y)
            );

            Vector3 target = transform.position + dir * config.gridSize;
            target = SnapToGrid(target);

            if (!IsGridBlocked(target))
            {
                _gridMoveTarget = target;
                _isGridMoving = true;
                IsMoving = true;
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        float g = config.gridSize;
        return new Vector3(
            Mathf.Round(pos.x / g) * g,
            pos.y,
            Mathf.Round(pos.z / g) * g
        );
    }

    private bool IsGridBlocked(Vector3 target)
    {
        // Raycast or overlap check for collisions on the grid
        return Physics.CheckSphere(target + Vector3.up * 0.5f, 0.2f,
                                    LayerMask.GetMask("Obstacles"));
    }

    // ================================================================
    //  FACING
    // ================================================================

    private void UpdateFacing(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            CurrentFacing = input.x > 0 ? FacingDirection.Right : FacingDirection.Left;
        else
            CurrentFacing = input.y > 0 ? FacingDirection.Up : FacingDirection.Down;
    }

    public void SetFacing(FacingDirection dir)
    {
        CurrentFacing = dir;
        UpdateAnimation();
    }

    // ================================================================
    //  ANIMATION
    // ================================================================
    
    private void UpdateAnimation()
    {
        /*
        if (_animator == null) return;

        Vector2 facingVec = FacingToVector(CurrentFacing);
        _animator.SetFloat(AnimMoveX, facingVec.x);
        _animator.SetFloat(AnimMoveY, facingVec.y);
        _animator.SetBool(AnimIsMoving, IsMoving);
        _animator.SetBool(AnimIsRunning, IsRunning && IsMoving);
        */
    }

    private Vector2 FacingToVector(FacingDirection dir) => dir switch
    {
        FacingDirection.Up    => Vector2.up,
        FacingDirection.Down  => Vector2.down,
        FacingDirection.Left  => Vector2.left,
        FacingDirection.Right => Vector2.right,
        _                     => Vector2.down
    };


    // ================================================================
    //  PUBLIC API
    // ================================================================

    public void SetEnabled(bool value) => _enabled = value;

    public void StopMovement()
    {
        IsMoving = false;
        _velocity = Vector3.zero;
        _isGridMoving = false;
        UpdateAnimation();
    }

    public void ResetDistance() => DistanceTravelledSinceReset = 0f;
}
