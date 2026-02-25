// PlayerInteractionHandler.cs
using UnityEngine;
using System.Collections.Generic;

public class PlayerWorldInteractionComponent : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
    }

    /*
    [SerializeField] private PlayerWorldConfig config;
    [SerializeField] private Transform interactionOrigin;

    private InteractableBase _currentTarget;
    private readonly Collider[] _overlapBuffer = new Collider[10];

    public InteractableBase CurrentTarget => _currentTarget;

    private void Awake()
    {
        if (interactionOrigin == null)
            interactionOrigin = transform;

        if (config == null)
            config = Resources.Load<PlayerWorldConfig>("PlayerWorldConfig");
    }

    /// <summary>Called every frame by PlayerWorldManager when in FreeRoam.</summary>
    public void Tick()
    {
        DetectInteractables();

        if (_currentTarget != null && Input.GetKeyDown(config.interactKey))
        {
            PlayerWorldManager.Instance.BeginInteraction(_currentTarget);
        }
    }

    private void DetectInteractables()
    {
        Vector3 origin = interactionOrigin.position;
        Vector3 facingDir = GetFacingVector();
        Vector3 castCenter = origin + facingDir * (config.interactionRange * 0.5f);

        int count = Physics.OverlapSphereNonAlloc(
            castCenter, config.interactionRange * 0.5f,
            _overlapBuffer, config.interactableLayer
        );

        InteractableBase closest = null;
        float closestDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var interactable = _overlapBuffer[i].GetComponent<InteractableBase>();
            if (interactable == null || !interactable.canInteract) continue;

            float dist = Vector3.Distance(origin, interactable.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        // State changes
        if (closest != _currentTarget)
        {
            _currentTarget?.OnPlayerLeftRange();
            _currentTarget = closest;
            _currentTarget?.OnPlayerInRange(transform);

            // Optionally show/hide interaction prompt UI
            InteractionPromptUI.Instance?.SetTarget(_currentTarget);
        }
    }



    */
}
