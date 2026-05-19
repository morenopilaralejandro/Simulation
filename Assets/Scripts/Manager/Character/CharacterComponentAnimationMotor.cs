using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Animation;

public class CharacterComponentAnimationMotor : MonoBehaviour
{
    #region Constants

    private const float MIN_MOVE_SQR = 0.01f;

    #endregion

    #region Serialized

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform model;
    [SerializeField] private CharacterComponentAnimationController animationController;

    [Header("Action Animations")]
    [SerializeField] private CharacterActionAnimationConfig actionConfig;

    #endregion

    #region Runtime

    private CharacterAnimationState locomotionState = CharacterAnimationState.Idle;
    private CharacterAnimationState currentActionState = CharacterAnimationState.Idle;
    private AnimationPriority currentPriority = AnimationPriority.Low;
    private bool hasAction;
    private float actionEndTime;
    private CharacterAnimationState currentResolvedState = CharacterAnimationState.Idle;
    private CharacterDirection currentDirection = CharacterDirection.Down;
    private Vector2 formationDirection;
    private Vector2 actionDirection;
    private bool hasActionDirection;

    #endregion

    #region Properties

    public bool IsPlayingAction => hasAction;
    public bool IsAnimationLocked =>
        hasAction &&
        actionConfig.TryGetData(currentActionState, out var data) &&
        data.LocksMovement;
    public void SetFormationDirection(Vector2 dir) => formationDirection = dir;

    #endregion

    #region Initialization

    private void Awake() 
    {
        actionConfig.BuildDatabase();
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        animationController =
            GetComponent<CharacterComponentAnimationController>();

        model = transform;
    }

    #endregion

    #region Public API

    public void SetLocomotion(CharacterAnimationState state)
    {
        locomotionState = state;
    }

    public bool RequestAction(
        CharacterAnimationState state,
        Vector2? direction = null)
    {
        if (!actionConfig.TryGetData(state, out var data))
            return false;

        if (hasAction)
        {
            if (!actionConfig.TryGetData(currentActionState, out var current))
                return false;

            if (!current.Interruptible && data.Priority <= currentPriority)
                return false;

            if (current.Interruptible && data.Priority < currentPriority)
                return false;
        }

        currentActionState = state;
        currentPriority = data.Priority;
        actionEndTime = Time.time + data.Duration;
        hasAction = true;

        hasActionDirection = direction.HasValue;
        actionDirection = direction ?? Vector2.zero;

        return true;
    }

    public void ForceAction(CharacterAnimationState state)
    {
        if (!actionConfig.TryGetData(state, out var data)) return;

        currentActionState = state;
        currentPriority = data.Priority;
        actionEndTime = Time.time + data.Duration;
        hasAction = true;
    }

    public void StopAction()
    {
        hasAction = false;
        currentActionState = CharacterAnimationState.Idle;
        currentPriority = AnimationPriority.Low;
    }

    #endregion

    #region Update

    public void OnLateUpdate()
    {
        UpdateActionState();
        ResolveAnimation();
    }

    private void UpdateActionState()
    {
        if (!hasAction || BattleManager.Instance.IsTimeFrozen || currentActionState == CharacterAnimationState.Hurt) return;

        if (Time.time >= actionEndTime)
        {
            StopAction();
        }
    }

    private void ResolveAnimation()
    {
        CharacterAnimationState state =
            hasAction ? currentActionState : locomotionState;

        actionConfig.TryGetData(state, out var data);

        CharacterDirection dir = ResolveDirection(data);

        if (state == currentResolvedState && dir == currentDirection) return;

        currentResolvedState = state;
        currentDirection = dir;

        animationController.Play(state, dir);
    }

    #endregion

    #region Direction

    private CharacterDirection ResolveDirection(ActionAnimationData data)
    {
        if (hasActionDirection) 
            return ResolveFromVector2(actionDirection);

        switch (data.FacingMode)
        {
            case AnimationFacingMode.Transform:
                return ResolveFromVector3(model.forward);

            case AnimationFacingMode.Formation:
                return ResolveFromVector2(formationDirection);

            case AnimationFacingMode.ActionOverride:
                if (hasActionDirection)
                    return ResolveFromVector2(actionDirection);

                return ResolveFromVector2(model.forward);

            case AnimationFacingMode.DownOnly:
                return CharacterDirection.Down;

            default:
                return ResolveFromVector3(model.forward);
        }
    }

    private CharacterDirection ResolveFromVector2(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0
                ? CharacterDirection.Right
                : CharacterDirection.Left;
        }

        return direction.y > 0
            ? CharacterDirection.Up
            : CharacterDirection.Down;
    }

    private CharacterDirection ResolveFromVector3(Vector3 f)
    {
        f.y = 0f;

        //if (f.sqrMagnitude < MIN_INPUT_SQR_MAGNITUDE) return cachedDirection;

        f.Normalize();

        if (Mathf.Abs(f.x) > Mathf.Abs(f.z))
            return f.x > 0 ? CharacterDirection.Left : CharacterDirection.Right;

        return f.z > 0 ? CharacterDirection.Up : CharacterDirection.Down;
    }

    #endregion
}
