using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer speechBubbleRenderer;

    private Coroutine hideRoutine;
    private float duration;

    private void Awake()
    {
        duration = SpeechBubbleManager.DEFAULT_DURATION;
        HideImmediate(null);
    }

    /// <summary>
    /// Shows a specific bubble message and auto-hides after the set duration.
    /// </summary>
    public void ShowMessage(SpeechMessage speechMessage, Character character)
    {
        // Stop any running routine
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        // Find the correct sprite
        speechBubbleRenderer.sprite = SpeechBubbleManager.Instance.GetSprite(speechMessage);
        speechBubbleRenderer.enabled = true;
        CharacterEvents.RaiseSpeechBubbleShown(character);

        hideRoutine = StartCoroutine(HideAfterSeconds(duration, character));
    }

    /// <summary>
    /// Instantly hides any visible bubble.
    /// </summary>
    public void HideImmediate(Character character)
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        speechBubbleRenderer.enabled = false;
        CharacterEvents.RaiseSpeechBubbleHidden(character);
    }

    private IEnumerator HideAfterSeconds(float seconds, Character character)
    {
        yield return new WaitForSeconds(seconds);
        HideImmediate(character);
    }
}
