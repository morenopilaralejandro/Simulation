using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BallComponentKinematic : MonoBehaviour
{
    private Ball ball;

    private float slowDuration = 1f;
    private AnimationCurve slowCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    private Coroutine slowingRoutine;
    [SerializeField] private Rigidbody ballRigidbody;

    public bool IsKinematic => ballRigidbody.isKinematic;
    public Vector3 GetVelocity() => ballRigidbody.velocity;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    public void SetKinematic()
    {
        /*
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        */
        ballRigidbody.isKinematic = true;
        ballRigidbody.useGravity = false;
    }

    public void SetDynamic()
    {
        ballRigidbody.isKinematic = false;
        ballRigidbody.useGravity = true;
    }

    public void SetDynamic(Vector3 velocity)
    {
        ballRigidbody.isKinematic = false;
        ballRigidbody.velocity = velocity;
        ballRigidbody.useGravity = true;
    }

    public void ToggleKinematic()
    {
        if (ballRigidbody.isKinematic) SetDynamic();
        else SetKinematic();
    }

    public void SlowDown()
    {
        if (slowingRoutine != null)
            StopCoroutine(slowingRoutine);

        slowingRoutine = StartCoroutine(SlowDownRoutine());
    }

    private IEnumerator SlowDownRoutine()
    {
        Vector3 initialVelocity = ballRigidbody.velocity;
        float elapsed = 0f;

        while (elapsed < slowDuration)
        {
            float t = elapsed / slowDuration;
            float curveValue = slowCurve.Evaluate(t);

            ballRigidbody.velocity = initialVelocity * curveValue;

            elapsed += Time.deltaTime;
            yield return null;
        }

        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }
}
