using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BannerZone : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text textName;
    private float fadeDuration = 0.25f;
    private float displayDuration = 1f;

    private Coroutine _displayCoroutine;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    #region Events

    private void OnEnable()
    {
        WorldEvents.OnZoneChanged += HandleZoneChanged;
    }

    private void OnDisable()
    {
        WorldEvents.OnZoneChanged -= HandleZoneChanged;
    }

    private void HandleZoneChanged(ZoneDefinition previousZone, ZoneDefinition newZone, string newName)
    {
        if (previousZone == newZone) return;

        if (_displayCoroutine != null)
            StopCoroutine(_displayCoroutine);

        _displayCoroutine = StartCoroutine(DisplayBannerCoroutine(newName));
    }

    #endregion

    private IEnumerator DisplayBannerCoroutine(string newName)
    {
        // If currently visible, fade out from current alpha before showing new text
        if (canvasGroup.alpha > 0f)
        {
            yield return Fade(canvasGroup.alpha, 0f);
        }

        // Set new text and fade in
        textName.text = newName;
        canvasGroup.blocksRaycasts = true;
        yield return Fade(0f, 1f);

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return Fade(1f, 0f);
        canvasGroup.blocksRaycasts = false;

        _displayCoroutine = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        // Scale duration proportionally to how much alpha actually needs to change
        // e.g. fading from 0.3 -> 0 only takes 30% of the full fadeDuration
        float distance = Mathf.Abs(to - from);
        float scaledDuration = fadeDuration * distance;

        if (scaledDuration <= 0f)
        {
            canvasGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < scaledDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / scaledDuration));
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
