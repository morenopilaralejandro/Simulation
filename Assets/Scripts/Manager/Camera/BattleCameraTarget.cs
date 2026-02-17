using UnityEngine;

public class BattleCameraTarget : MonoBehaviour
{
    // Offsets
    private readonly Vector3 homeOffset = new Vector3(0f, 3f, -3.8f);
    private readonly Vector3 awayOffset = new Vector3(0f, 3f, -6f);

    // Smoothing
    private const float NormalSmoothSpeed = 5.5f;
    private const float FastSmoothSpeed = 100f;
    private const float DistanceThresholdSqr = 15f * 15f; // squared to avoid sqrt

    // Cached references (re-resolved once per frame block, not per property call)
    private BattleManager _battleMgr;
    private PossessionManager _possessionMgr;
    private BoundManager _boundMgr;

    private Vector3 _offset;
    private Transform _transform; // cache own transform

    private void Awake()
    {
        _transform = transform;
        _offset = awayOffset;
    }

    private void Start()
    {
        // Cache singletons once; they persist for the scene lifetime
        _battleMgr = BattleManager.Instance;
        _possessionMgr = PossessionManager.Instance;
        _boundMgr = BoundManager.Instance;
    }

    private void LateUpdate()
    {
        // Early-out: cache ball reference once
        var ball = _battleMgr.Ball;
        if (ball == null) return;

        bool isTraveling = ball.IsTraveling;
        Transform followTarget;

        // ── Decide who to follow ──
        // NOTE: Original had an operator-precedence bug.
        // Fixed: parentheses now match the intended logic.
        var current = _possessionMgr.CurrentCharacter;
        bool followHome;

        if (current != null)
        {
            followHome = current.IsOnUsersTeam() && !isTraveling;
        }
        else
        {
            var last = _possessionMgr.LastCharacter;
            followHome = last != null && last.IsOnUsersTeam() && !isTraveling;
        }

        if (followHome)
        {
            _offset = homeOffset;
            followTarget = _battleMgr.ControlledCharacter[_battleMgr.GetUserSide()].transform;
        }
        else
        {
            if (!isTraveling)
                _offset = awayOffset;

            followTarget = ball.transform;
        }

        // ── Compute desired position ──
        Vector3 desired;
        desired.x = followTarget.position.x + _offset.x;
        desired.y = followTarget.position.y + _offset.y;
        desired.z = followTarget.position.z + _offset.z;

        Vector3 clamped = _boundMgr.ClampCamera(desired);

        // ── Adaptive smooth speed (sqr magnitude avoids sqrt) ──
        float dx = _transform.position.x - clamped.x;
        float dy = _transform.position.y - clamped.y;
        float dz = _transform.position.z - clamped.z;
        float sqrDist = dx * dx + dy * dy + dz * dz;

        float speed = sqrDist > DistanceThresholdSqr ? FastSmoothSpeed : NormalSmoothSpeed;
        float t = Time.deltaTime * speed;

        // ── Lerp manually (avoids Vector3.Lerp method-call overhead) ──
        Vector3 pos = _transform.position;
        pos.x += (clamped.x - pos.x) * t;
        pos.y += (clamped.y - pos.y) * t;
        pos.z += (clamped.z - pos.z) * t;

        _transform.position = pos;
    }
}
