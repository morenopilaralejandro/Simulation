using UnityEngine;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.World;

public class PlayerWorldComponentInteraction : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    private Transform interactionOrigin;

    private Transform _cachedTransform;
    private int _frameCounter;
    private IInteractable _currentTarget;
    private RaycastHit2D _hitInfo2D;

    public IInteractable CurrentTarget => _currentTarget;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
    }

    private void Awake()
    {
        _cachedTransform = transform;

        if (interactionOrigin == null)
            interactionOrigin = _cachedTransform;
    }

    public void OnUpdate()
    {
        if (!playerWorldEntity.CanInteract) return;

        _frameCounter++;
        if (_frameCounter >= config.detectionInterval)
        {
            _frameCounter = 0;
            DetectInteractable();
        }

        if (_currentTarget != null && InputManager.Instance.GetDown(CustomAction.World_Interact))
        {
            _currentTarget.Interact();
        }
    }

    private void DetectInteractable()
    {
        Vector3 direction = playerWorldEntity.FacingToVector(playerWorldEntity.FacingDirection);

        _hitInfo2D = Physics2D.CircleCast(
            interactionOrigin.position,
            config.castRadius,
            direction,
            config.interactionRange,
            config.interactableLayer
        );

        if (_hitInfo2D.collider != null)
        {
            IInteractable interactable = _hitInfo2D.collider
                .GetComponent<InteractableComponentCollider>()
                .Interactable;

            if (interactable != null)
            {
                _currentTarget = interactable;
                return;
            }
        }

        _currentTarget = null;
    }

    private void OnDisable()
    {
        _currentTarget = null;
        _frameCounter = 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (config == null) return;

        Transform origin = interactionOrigin != null ? interactionOrigin : transform;

        Vector3 direction = Vector3.right;

        if (Application.isPlaying && playerWorldEntity != null)
        {
            direction = playerWorldEntity.FacingToVector(playerWorldEntity.FacingDirection);
        }

        Vector3 start = origin.position;
        Vector3 end = start + (direction.normalized * config.interactionRange);

        // Detection path
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);

        // Start circle
        Gizmos.DrawWireSphere(start, config.castRadius);

        // End circle
        Gizmos.DrawWireSphere(end, config.castRadius);

        // Hit point
        if (Application.isPlaying && _hitInfo2D.collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_hitInfo2D.point, 0.08f);
        }
    }
#endif
}
