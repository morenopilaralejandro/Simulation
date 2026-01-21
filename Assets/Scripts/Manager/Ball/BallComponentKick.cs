using UnityEngine;

public class BallComponentKick : MonoBehaviour
{
    private Ball ball;

    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private LayerMask characterPresenceLayer;

    private string tagCharacterPresence = "Character-Presence";
    private float shortDistance = 10f;
    private float mediumDistance = 25f;
    private float maxPower = 25f;
    private float gravity = 9.81f;
    private static readonly Collider[] overlapResults = new Collider[6];
    private float sphereCastRadius = 0.4f;
    private float opponentCheckStartOffset = 0.6f;
    //private float castHeightOffset = 0f;

    #if UNITY_EDITOR
    [SerializeField] private bool debugDraw = true;
    private Vector3 debugCastStart;
    private Vector3 debugCastEnd;
    private bool debugHadHit;
    private Vector3 debugHitPoint;
    #endif

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
            angle += 45f; // lift trajectory to go over opponent

        angle = Mathf.Clamp(angle, 10f, 45f);

        Vector3 velocity = CalculateBallisticVelocity(targetPos, ballPos, angle);

        // Clamp total power to limit "arcade realism"
        if (velocity.magnitude > maxPower)
            velocity = velocity.normalized * maxPower;

        if (!ball.IsFree())
            PossessionManager.Instance.Release();
        ballRigidbody.velocity = velocity;

        AudioManager.Instance.PlaySfx("sfx-ball_kick");
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
        float castHeight = ballPos.y;

        Vector3 direction = (targetPos - ballPos).normalized;

        //Vector3 start = new Vector3(ballPos.x, castHeight, ballPos.z);
        Vector3 start = ballPos + direction * opponentCheckStartOffset;
        Vector3 end   = new Vector3(targetPos.x, castHeight, targetPos.z);

    #if UNITY_EDITOR
        debugCastStart = start;
        debugCastEnd = end;
        debugHadHit = false;
    #endif

        int count = Physics.OverlapCapsuleNonAlloc(
            start,
            end,
            sphereCastRadius,
            overlapResults,
            characterPresenceLayer,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < count; i++)
        {
            Collider col = overlapResults[i];

            if(!col.gameObject.CompareTag(tagCharacterPresence)) continue;
            var presence = col.GetComponent
                <CharacterComponentColliderPresence>().
                Character;

            if (presence == null || presence.IsStunned()) continue;

            Character kickCharacter = 
                PossessionManager.Instance.CurrentCharacter == null ?
                    PossessionManager.Instance.LastCharacter :
                    PossessionManager.Instance.CurrentCharacter;

            if (kickCharacter && !presence.IsSameTeam(kickCharacter))
            {
    #if UNITY_EDITOR
                debugHadHit = true;
                debugHitPoint = col.ClosestPoint((start + end) * 0.5f);
    #endif
                return true;
            }
        }

        return false;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!debugDraw) return;

        Gizmos.color = Color.yellow;

        // Main capsule axis
        Gizmos.DrawLine(debugCastStart, debugCastEnd);

        // End spheres
        Gizmos.DrawWireSphere(debugCastStart, sphereCastRadius);
        Gizmos.DrawWireSphere(debugCastEnd, sphereCastRadius);

        // Side edges (visual aid)
        Vector3 dir = (debugCastEnd - debugCastStart).normalized;
        Vector3 right = Vector3.Cross(dir, Vector3.up) * sphereCastRadius;

        Gizmos.DrawLine(debugCastStart + right, debugCastEnd + right);
        Gizmos.DrawLine(debugCastStart - right, debugCastEnd - right);

        if (debugHadHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(debugHitPoint, 0.15f);
        }
    }
    #endif  
}
