using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class BallComponentKinematic : MonoBehaviour
{
    private Ball ball;

    private float slowDuration = 1f;
    private AnimationCurve slowCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    private Coroutine slowingRoutine;
    private Vector3 cachedVelocity;
    private Vector3 cachedAngularVelocity;
    private Vector3 cachedPosition;
    private Quaternion cachedRotation;

    [SerializeField] private Rigidbody ballRigidbody;

    public bool IsKinematic => ballRigidbody.isKinematic;
    public Vector3 GetVelocity() => ballRigidbody.velocity;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    void OnEnable()
    {
        BattleEvents.OnPauseBattle += HandlePausedBattle;
        BattleEvents.OnResumeBattle += HandleResumeBattle;
    }

    void OnDisable()
    {
        BattleEvents.OnPauseBattle -= HandlePausedBattle;
        BattleEvents.OnResumeBattle -= HandleResumeBattle;
    }

    private void HandlePausedBattle()
    {
        PausePhysics();
    }

    private void HandleResumeBattle()
    {
        ResumePhysics();
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

    public void PausePhysics()
    {
        if (ballRigidbody.isKinematic) return;

        cachedVelocity = ballRigidbody.velocity;
        cachedAngularVelocity = ballRigidbody.angularVelocity;

        ballRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }

    public void ResumePhysics()
    {
        if (ballRigidbody.isKinematic) return;

        ballRigidbody.constraints = RigidbodyConstraints.None;

        ballRigidbody.velocity = cachedVelocity;
        ballRigidbody.angularVelocity = cachedAngularVelocity;
    }

    public void ResetPhysics() 
    {
        SetDynamic();
        ballRigidbody.constraints = RigidbodyConstraints.None;

        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }

    public void SlowDown()
    {
        if (slowingRoutine != null)
            StopCoroutine(slowingRoutine);

        slowingRoutine = StartCoroutine(SlowDownRoutine());
    }

    private IEnumerator SlowDownRoutine()
    {
        float elapsed = 0f;
        Vector3 dir = ballRigidbody.velocity.normalized;
        float speed = ballRigidbody.velocity.magnitude;
        while (elapsed < slowDuration)
        {
            float t = elapsed / slowDuration;
            float curveValue = slowCurve.Evaluate(t);
            ballRigidbody.velocity = dir * (speed * curveValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }
}
