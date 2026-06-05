using UnityEngine;
using Aremoreno.Enums.Move;

public class MoveCutsceneHideRenderer : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;

    private void OnEnable()
    {
        MoveEvents.OnMoveCutsceneStart += HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd += HandleMoveCutsceneEnd;

        WingEvents.OnWingCutsceneStart += HandleWingCutsceneStart;
        WingEvents.OnWingCutsceneEnd += HandleWingCutsceneEnd;
    }

    private void OnDisable()
    {
        MoveEvents.OnMoveCutsceneStart -= HandleMoveCutsceneStart;
        MoveEvents.OnMoveCutsceneEnd -= HandleMoveCutsceneEnd;

        WingEvents.OnWingCutsceneStart -= HandleWingCutsceneStart;
        WingEvents.OnWingCutsceneEnd -= HandleWingCutsceneEnd;
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

    private void HandleWingCutsceneStart(Wing wing)
    {
        if (targetRenderer != null)
        {
            targetRenderer.enabled = true;
        }
    }

    private void HandleWingCutsceneEnd()
    {
        if (targetRenderer != null)
        {
            targetRenderer.enabled = false;
        }
    }
}
