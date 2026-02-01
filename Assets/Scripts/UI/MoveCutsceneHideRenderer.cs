using UnityEngine;
using Simulation.Enums.Move;

public class MoveCutsceneHideRenderer : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    private void OnEnable()
    {
        MoveEvents.OnMoveCutsceneStart += HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd += HandleMoveCutsceneEnd;
    }

    private void OnDisable()
    {
        MoveEvents.OnMoveCutsceneStart -= HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd -= HandleMoveCutsceneEnd;
    }

    private void HandleMoveCutsceneStart(Move move)
    {
        if (targetRenderer != null)
        {
            targetRenderer.enabled = true;
        }
    }

    private void HandleMoveCutsceneEnd()
    {
        if (targetRenderer != null)
        {
            targetRenderer.enabled = false;
        }
    }
}
