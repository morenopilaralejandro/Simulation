using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.World;

public class PlayerWorldComponentInteraction : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    private Transform interactionOrigin;

    private Transform _cachedTransform;
    private int _frameCounter;
    private Interactable _currentTarget;
    private RaycastHit2D _hitInfo2D;

    public Interactable CurrentTarget => _currentTarget;

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

    private void Update()
    {
        if (playerWorldEntity == null) return;
        if (!playerWorldEntity.IsEnabled) return;
        if (playerWorldEntity.PlayerWorldState != PlayerWorldState.FreeRoam) return;

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
            Interactable interactable = _hitInfo2D.collider.GetComponent<InteractableComponentCollider>().Interactable;

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

}
