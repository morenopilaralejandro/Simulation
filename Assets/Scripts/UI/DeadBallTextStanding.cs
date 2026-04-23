using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.DeadBall;

public class DeadBallTextStanding : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        DeadBallEvents.OnDeadBallStarted += HandleDeadBallStarted;
        DeadBallEvents.OnDeadBallReady += HandleDeadBallReady;
    }

    private void OnDisable()
    {
        DeadBallEvents.OnDeadBallStarted -= HandleDeadBallStarted;
        DeadBallEvents.OnDeadBallReady -= HandleDeadBallReady;
    }

    private void HandleDeadBallStarted(DeadBallType type, TeamSide offenseSide, TeamSide defenseSide)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private void HandleDeadBallReady(DeadBallType type, TeamSide offenseSide, TeamSide defenseSide)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }
}
