using UnityEngine;

public class BallComponentKick : MonoBehaviour
{
    private Ball ball;

    [SerializeField] private Rigidbody ballRigidbody;

    private float shortDistance = 10f;
    private float mediumDistance = 25f;
    private float maxPower = 25f;
    private float gravity = 9.81f;
    private float sphereCastRadius = 0.3f;
    private string tagCharacterPresence = "Character-Presence";

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    public void KickBallTo(Vector3 targetPos)
    {
        Vector3 ballPos = ball.transform.position;
        Vector3 toTarget = targetPos - ballPos;

        float distance = new Vector2(toTarget.x, toTarget.z).magnitude;
        bool isOpponentInWay = IsOpponentInWay(ballPos, targetPos);

        // Adjust angle based on distance and opponent presence
        float angle = 15f; // base low pass
        if (distance > shortDistance && distance <= mediumDistance)
            angle = 25f;
        else if (distance > mediumDistance)
            angle = 35f;

        if (isOpponentInWay)
            angle += 10f; // lift trajectory to go over opponent

        angle = Mathf.Clamp(angle, 10f, 45f);

        Vector3 velocity = CalculateBallisticVelocity(targetPos, ballPos, angle);

        // Clamp total power to limit "arcade realism"
        if (velocity.magnitude > maxPower)
            velocity = velocity.normalized * maxPower;

        PossessionManager.Instance.Release();
        ballRigidbody.velocity = velocity;
    }

    private Vector3 CalculateBallisticVelocity(Vector3 target, Vector3 origin, float launchAngle)
    {
        float radianAngle = launchAngle * Mathf.Deg2Rad;
        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarOrigin = new Vector3(origin.x, 0, origin.z);

        float distance = Vector3.Distance(planarTarget, planarOrigin);
        float yOffset = origin.y - target.y;

        float velocity = Mathf.Sqrt((distance * gravity) / (Mathf.Sin(2 * radianAngle)));

        Vector3 direction = (planarTarget - planarOrigin).normalized;
        Vector3 velocityVec = direction * (velocity * Mathf.Cos(radianAngle));
        velocityVec.y = velocity * Mathf.Sin(radianAngle);

        return velocityVec;
    }

    private bool IsOpponentInWay(Vector3 ballPos, Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - ballPos;
        float distance = toTarget.magnitude;

        // Cast a small sphere along the path to account for ball width
        RaycastHit[] hits = Physics.SphereCastAll(
            ballPos + Vector3.up * 0.5f,
            sphereCastRadius, 
            toTarget.normalized,
            distance
        );

        foreach (RaycastHit hit in hits)
        {
            GameObject obj = hit.collider.gameObject;

            // Skip self or the ball
            if (obj == gameObject || obj == ball.gameObject || !obj.CompareTag(tagCharacterPresence))
                continue;

            Character character = obj.GetComponent<Character>();
            if (character != null && !character.IsSameTeam(PossessionManager.Instance.LastCharacter))
                return true;
        }

        return false;
    }
}
