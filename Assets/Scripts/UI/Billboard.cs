using UnityEngine;
using Cinemachine;

public class Billboard : MonoBehaviour
{
    public Camera targetCamera; // If left null, will auto-use Camera.main
    public bool keepUpright = true;

    void Start()
    {
        if (targetCamera == null)
        {
            // Try to find the main camera or one with a Cinemachine Brain
            var brain = FindObjectOfType<CinemachineBrain>();
            if (brain != null)
                targetCamera = brain.OutputCamera;
            else
                targetCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 lookDirection = transform.position - targetCamera.transform.position;

        if (keepUpright)
        {
            Vector3 lookPos = targetCamera.transform.position;
            lookPos.y = transform.position.y; // Lock vertical
            transform.LookAt(lookPos, Vector3.up);
        }
        else
        {
            transform.LookAt(targetCamera.transform);
        }
    }
}
