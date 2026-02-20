using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

/// <summary>
/// Simple fade-to-black transition screen.
/// </summary>
public class TransitionScreen : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public Image fadeImage;

    [Header("Settings")]
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public async Task FadeOut()
    {
        canvasGroup.blocksRaycasts = true;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            await Task.Yield();
        }

        canvasGroup.alpha = 1f;
    }

    public async Task FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            await Task.Yield();
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
