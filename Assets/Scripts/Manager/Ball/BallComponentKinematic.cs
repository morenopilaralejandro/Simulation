using UnityEngine;
using System.Collections;
using Simulation.Enums.Character;

[RequireComponent(typeof(Rigidbody))]
public class BallComponentKinematic : MonoBehaviour
{
    private Ball ball;

    private float slowDuration = 1f;
    private AnimationCurve slowCurve = new AnimationCurve(
        new Keyframe(0f, 1f, 0f, -1f),
        new Keyframe(1f, 0f, -0.1f, 0f)
    );
    private Coroutine slowingRoutine;
    private Vector3 cachedVelocity;
    private Vector3 cachedAngularVelocity;
    private Vector3 cachedPosition;
    private Quaternion cachedRotation;

    [SerializeField] private Rigidbody ballRigidbody;

    public bool IsKinematic => ballRigidbody.isKinematic;
    public Vector3 GetVelocity() => ballRigidbody.velocity;

    void Awake()
    {
        StartCoroutine(WarmUpBall());
    }

    IEnumerator WarmUpBall()
    {
        ballRigidbody.WakeUp();
        ballRigidbody.velocity = Vector3.zero;

        // Force one physics step worth of contact generation
        ballRigidbody.AddForce(Vector3.up * 1f, ForceMode.VelocityChange);
        yield return new WaitForFixedUpdate();
    }

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    void OnEnable()
    {
        BattleEvents.OnBattlePause += HandleBattlePause;
        BattleEvents.OnBattleResume += HandleBattleResume;
    }

    void OnDisable()
    {
        BattleEvents.OnBattlePause -= HandleBattlePause;
        BattleEvents.OnBattleResume -= HandleBattleResume;
    }

    private void HandleBattlePause(TeamSide teamSide) => PausePhysics();
    private void HandleBattleResume() => ResumePhysics();

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
        Vector3 initialVelocity = ballRigidbody.velocity;
   
        while (elapsed < slowDuration)
        {
            float t = elapsed / slowDuration;
            float curveValue = slowCurve.Evaluate(t);

            if (!ballRigidbody.isKinematic)
                ballRigidbody.velocity *= curveValue;


            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }
}
