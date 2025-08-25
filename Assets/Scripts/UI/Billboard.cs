using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera targetCamera; // If left null, will auto-use Camera.main
    public bool keepUpright = true;

    void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

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
