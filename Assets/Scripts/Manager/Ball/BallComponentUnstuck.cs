using UnityEngine;
using Aremoreno.Enums.Battle;

public class BallComponentUnstuck : MonoBehaviour
{
    private Ball ball;

    [SerializeField] private Rigidbody rb;
    private float pushForce = 0.1f;
    private float stuckTimeRequired = 1f;
    private float stuckSpeedThreshold = 0.5f;
    private float checkInterval = 0.25f;
    private float unstuckCooldown = 1f;
    private float displacementDistance = 0.1f;

    private float stuckTimer;
    private float checkTimer;
    private float cooldownTimer;
    private int unstuckAttempts;
    private bool isTouchingBorder;
    private Vector3 lastContactNormal;
    private float sqrSpeedThreshold;

    private static readonly Vector3 Origin = Vector3.zero;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;

        if (BattleManager.Instance.CurrentType == BattleType.Full)
            this.enabled = false;
    }

    private void Awake()
    {
        sqrSpeedThreshold = stuckSpeedThreshold * stuckSpeedThreshold;
    }

    private void OnEnable()
    {
        stuckTimer = 0f;
        checkTimer = 0f;
        cooldownTimer = 0f;
        unstuckAttempts = 0;
        isTouchingBorder = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Bound"))
            return;

        isTouchingBorder = true;
        lastContactNormal = collision.contacts[0].normal;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Bound"))
            return;

        isTouchingBorder = true;
        lastContactNormal = collision.contacts[0].normal;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.collider.CompareTag("Bound"))
            return;

        isTouchingBorder = false;
        stuckTimer = 0f;
        unstuckAttempts = 0;
    }

    private void FixedUpdate()
    {
        if (ball.IsKinematic)
            return;

        // Respect cooldown after an unstuck attempt
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!isTouchingBorder)
        {
            unstuckAttempts = 0;
            return;
        }

        checkTimer += Time.fixedDeltaTime;
        if (checkTimer < checkInterval)
            return;

        checkTimer = 0f;

        if (rb.velocity.sqrMagnitude > sqrSpeedThreshold)
        {
            stuckTimer = 0f;
            return;
        }

        stuckTimer += checkInterval;

        if (stuckTimer >= stuckTimeRequired)
        {
            Unstuck();
        }
    }

    private void Unstuck()
    {
        unstuckAttempts++;

        Vector3 toCenter = Origin - transform.position;
        toCenter.y = 0f;

        Vector3 pushDir;

        if (toCenter.sqrMagnitude > 0.01f)
        {
            pushDir = toCenter.normalized;
        }
        else
        {
            // Already at center, use contact normal to push away from wall
            pushDir = lastContactNormal;
            pushDir.y = 0f;
            pushDir.Normalize();
        }

        // Escalate force and displacement on repeated failures
        float scaledDisplacement = displacementDistance * unstuckAttempts;
        float scaledForce = pushForce * unstuckAttempts;

        // Physically displace the ball off the wall surface
        rb.position += pushDir * scaledDisplacement;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(pushDir * scaledForce, ForceMode.VelocityChange);

        stuckTimer = 0f;
        isTouchingBorder = false;
        cooldownTimer = unstuckCooldown;
    }
}
