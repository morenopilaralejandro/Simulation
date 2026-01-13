using UnityEngine;

public class BallShadow : MonoBehaviour
{
    private Ball ball;

    [Header("Shadow Settings")]
    private float groundY = 0.02f;
    private float maxShadowScale = 0.7f;
    private float minShadowScale = 0.3f;
    private float maxHeight = 3f;

    private Vector3 baseScale;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        baseScale = transform.localScale;
        meshRenderer = GetComponent<MeshRenderer>();

        BallEvents.OnBallSpawned += HandleBallSpawned;
    }

    void Destroy()
    {
        BallEvents.OnBallSpawned -= HandleBallSpawned;
    }

    private void HandleBallSpawned(Ball ball) 
    {
        this.ball = ball;
    }

    void LateUpdate()
    {
        // Follow ball on X/Z ONLY
        Vector3 pos = transform.position;
        pos.x = ball.transform.position.x;
        pos.z = ball.transform.position.z;
        pos.y = groundY;
        transform.position = pos;

        // Calculate height
        float height = Mathf.Clamp(ball.transform.position.y - groundY, 0f, maxHeight);
        float t = 1f - (height / maxHeight);

        // Scale shadow
        float scale = Mathf.Lerp(minShadowScale, maxShadowScale, t);
        transform.localScale = baseScale * scale;

        // Lock rotation
        //transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
