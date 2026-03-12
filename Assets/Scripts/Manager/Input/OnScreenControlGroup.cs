using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to each control group root (OnScreenJoystick, OnScreenDpad, etc.).
/// Stores its own default/current scale & opacity and applies them cleanly.
/// </summary>
public class OnScreenControlGroup : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField] private float defaultScale = 1f;
    [SerializeField] private float defaultOpacity = 0.5f;

    [Header("Runtime (read-only in inspector)")]
    [SerializeField, Range(0.01f, 2f)] private float currentScale;
    [SerializeField, Range(0f, 1f)] private float currentOpacity;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public float CurrentScale => currentScale;
    public float CurrentOpacity => currentOpacity;
    public float DefaultScale => defaultScale;
    public float DefaultOpacity => defaultOpacity;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // CanvasGroup is the proper Unity way to control opacity for an entire subtree
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Initialize to defaults
        ApplyScale(defaultScale);
        ApplyOpacity(defaultOpacity);
    }

    /// <summary>Set scale (0.01–2.0 range).</summary>
    public void SetScale(float scale)
    {
        currentScale = Mathf.Clamp(scale, 0.01f, 2f);
        ApplyScale(currentScale);
    }

    /// <summary>Set opacity (0–1 range). Uses CanvasGroup so it doesn't touch individual Image.color.</summary>
    public void SetOpacity(float alpha)
    {
        currentOpacity = Mathf.Clamp01(alpha);
        ApplyOpacity(currentOpacity);
    }

    public void ResetScale() => SetScale(defaultScale);
    public void ResetOpacity() => SetOpacity(defaultOpacity);

    public void ResetAll()
    {
        ResetScale();
        ResetOpacity();
    }

    private void ApplyScale(float scale)
    {
        currentScale = scale;
        if (rectTransform != null)
            rectTransform.localScale = Vector3.one * scale;
    }

    private void ApplyOpacity(float alpha)
    {
        currentOpacity = alpha;
        if (canvasGroup != null)
            canvasGroup.alpha = alpha;
    }
}
