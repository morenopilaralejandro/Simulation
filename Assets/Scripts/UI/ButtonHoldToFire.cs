using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ButtonHoldToFire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button button;
    private float initialDelay = 0.4f;
    private float repeatInterval = 0.1f;
    private string sfxId = "sfx-menu_tap";

    private Coroutine holdCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        holdCoroutine = StartCoroutine(HoldRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopHold();
    }

    private IEnumerator HoldRoutine()
    {
        // Normal click happens immediately
        //button.onClick.Invoke();

        // Wait before starting repeated clicks
        yield return new WaitForSeconds(initialDelay);

        // Keep invoking while held
        while (true)
        {
            AudioManager.Instance.PlaySfxUI(sfxId);
            button.onClick.Invoke();
            yield return new WaitForSeconds(repeatInterval);
        }
    }

    private void StopHold()
    {
        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;
        }
    }

    private void OnDisable()
    {
        StopHold();
    }
}
