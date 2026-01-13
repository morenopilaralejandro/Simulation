using UnityEngine;

public class BattleCameraTarget : MonoBehaviour
{
    [Header("Offsets")]
    private Vector3 offset = new Vector3(0, 3, -6);
    private Vector3 homeOffset = new Vector3(0, 3, -3.8f);
    private Vector3 awayOffset = new Vector3(0, 3, -6);

    [Header("Smoothing")]
    private float currentSmoothSpeed = 2f;
    private float normalSmoothSpeed = 5.5f;
    private float fastSmoothSpeed = 100f;
    private float distanceThreshold = 15f;

    void LateUpdate()
    {
        Transform followTarget;

        // Decide who to follow depending on possession
        if (PossessionManager.Instance.CurrentCharacter != null && 
            PossessionManager.Instance.CurrentCharacter.IsOnUsersTeam() || 
            (PossessionManager.Instance.CurrentCharacter == null && 
             PossessionManager.Instance.LastCharacter != null &&
             PossessionManager.Instance.LastCharacter.IsOnUsersTeam()) && 
            !BattleManager.Instance.Ball.IsTraveling) 
        {
            offset = homeOffset;
            followTarget = BattleManager.Instance.ControlledCharacter[BattleTeamManager.Instance.GetUserSide()].transform;
        }
        else
        {
            if (!BattleManager.Instance.Ball.IsTraveling)
                offset = awayOffset;
            followTarget = BattleManager.Instance?.Ball.transform;
        }

        // Base target position = followed object position + correct offset
        Vector3 desiredPosition = followTarget.position;
        desiredPosition += offset;

        // Clamp to field boundaries
        Vector3 clamped = BoundManager.Instance.ClampCamera(desiredPosition);

        float distance = Vector3.Distance(transform.position, clamped);
        currentSmoothSpeed = distance > distanceThreshold ? fastSmoothSpeed : normalSmoothSpeed;

        // Smoothly move camera
        transform.position = Vector3.Lerp(
            transform.position,
            clamped,
            Time.deltaTime * currentSmoothSpeed
        );
    }
}
