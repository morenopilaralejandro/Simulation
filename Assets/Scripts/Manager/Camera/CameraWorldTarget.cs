using UnityEngine;
using Cinemachine;

public class CameraWorldTarget : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineBrain cinemachineBrain;

    private void Start()
    {
        Transform playerTransform = WorldManager.Instance.PlayerWorldEntity.transform;
        virtualCamera.Follow = playerTransform;
        virtualCamera.LookAt = playerTransform;

        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
    }

    private void OnEnable()
    {
        WorldEvents.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnDisable()
    {
        WorldEvents.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private void OnPlayerTeleported(Vector3 position)
    {
        transform.position = position;

        // --- Force the camera to snap immediately ---

        // 1. Reset the Cinemachine Body's cached state so damping doesn't apply
        virtualCamera.PreviousStateIsValid = false;

        // 2. Force the brain to update immediately (cuts through any blending)
        cinemachineBrain.ManualUpdate();
    }
}
