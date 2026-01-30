using UnityEngine;
using System.Collections;

public class PassCrosshairIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer indicatorRenderer;
    private float displayDuration = 1f;

    private Coroutine hideCoroutine;

    private void Start() 
    {
        BattleUIManager.Instance.RegisterPassCrosshairIndicator(this);
        BallEvents.OnGained += OnBallGained;
    }

    private void OnDestroy() 
    {
        BattleUIManager.Instance.UnregisterPassCrosshairIndicator(this);
        BallEvents.OnGained -= OnBallGained;
    }

    public void ShowAtPosition(Vector3 worldPosition)
    {
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(
            worldPosition.x,
            currentPosition.y,
            worldPosition.z
        );

        indicatorRenderer.enabled = true;

        if (hideCoroutine != null) 
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        indicatorRenderer.enabled = false;
        hideCoroutine = null;
    }

    private void OnBallGained(Character character)
    {
        HideImmediately();
    }

    public void HideImmediately()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        indicatorRenderer.enabled = false;
    }
}
