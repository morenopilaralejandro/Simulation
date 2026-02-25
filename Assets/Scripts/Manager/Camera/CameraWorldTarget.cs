using UnityEngine;
using Cinemachine;

public class CameraWorldTarget : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {

    }

    private void Start()
    {
        GameObject player = WorldManager.Instance.PlayerWorldEntity.gameObject;
        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
    }

}
