// PlayerMovementController.cs
using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.World;

[RequireComponent(typeof(CharacterController))]
public class PlayerWorldControllerComponent : MonoBehaviour
{
    [SerializeField] private CharacterController _cc;

    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    public bool IsMoving { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public float DistanceTravelledSinceReset { get; private set; }

    private bool _enabled = false;
    private Vector3 _velocity;
    private bool _isGridMoving;
    private Vector3 _gridMoveTarget;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
    }

    private void Update()
    {
        if (!_enabled) return;

        float deltaTime = Time.deltaTime;

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
        /*
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (!config.allowDiagonalMovement)
        {
            // Prioritise horizontal if both pressed
            if (Mathf.Abs(h) > 0.01f && Mathf.Abs(v) > 0.01f)
                v = 0f;
        }
        */

        MoveInput = InputManager.Instance.GetMove();
        IsRunning = !InputManager.Instance.GetHeld(CustomAction.Battle_Pass); //auto run by default and walk when held
    }

    // ================================================================
    //  FREE MOVEMENT (analog)
    // ================================================================

    private void HandleFreeMovement(float dt)
    {
        Vector3 direction = new Vector3(MoveInput.x, MoveInput.y, 0f).normalized;
        float speed = IsRunning ? config.runSpeed : config.walkSpeed;

        IsMoving = direction.sqrMagnitude > 0.01f;

        if (IsMoving)
        {
            playerWorldEntity.SetFacing(MoveInput);

            _velocity = direction * speed;

            /*
            // Apply gravity
            if (!_cc.isGrounded)
                _velocity.y -= 9.81f * dt;
            */

            _cc.Move(_velocity * dt);
            DistanceTravelledSinceReset += speed * dt;
        }
        else
        {
            /*
            // Still apply gravity
            if (!_cc.isGrounded)
            {
                _velocity.y -= 9.81f * dt;
                _cc.Move(_velocity * dt);
            }
            */
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
            Vector3 newPos = Vector3.MoveTowards(transform.position, _gridMoveTarget, speed * dt);
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
            playerWorldEntity.SetFacing(MoveInput);

            Vector3 dir = new Vector3(
                Mathf.Round(MoveInput.x),
                Mathf.Round(MoveInput.y),
                0f
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

    // ================================================================
    //  PUBLIC API
    // ================================================================

    public void SetControlEnabled(bool enabled) => _enabled = enabled;

    public void StopMovement()
    {
        IsMoving = false;
        _velocity = Vector3.zero;
        _isGridMoving = false;
        UpdateAnimation();
    }

    public void ResetDistance() => DistanceTravelledSinceReset = 0f;
}
