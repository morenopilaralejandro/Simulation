using UnityEngine;
using Cinemachine;

public sealed class Billboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool keepUpright = true;

    private Transform cachedTransform;
    private static Camera cachedMainCamera;

    private void Awake()
    {
        cachedTransform = transform;

        if (targetCamera == null)
        {
            if (cachedMainCamera == null)
            {
                var brain = FindFirstObjectByType<CinemachineBrain>();
                if (brain != null && brain.OutputCamera != null)
                {
                    cachedMainCamera = brain.OutputCamera;
                }
                else
                {
                    cachedMainCamera = Camera.main;
                }
            }

            targetCamera = cachedMainCamera;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 toCamera = targetCamera.transform.position - cachedTransform.position;

        if (keepUpright) toCamera.y = 0f;

        if (toCamera.sqrMagnitude < 0.0001f) return;

        cachedTransform.rotation = Quaternion.LookRotation(-toCamera, Vector3.up);
    }
}
