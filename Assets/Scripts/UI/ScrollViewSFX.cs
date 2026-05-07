using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewSFX : MonoBehaviour
{
    [Tooltip("Minimum scroll delta before playing sound")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollThreshold = 0.02f;

    private Vector2 lastPosition;
    private string scrollSfxId = "sfx-menu_scroll";

    void Awake()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);
        lastPosition = scrollRect.normalizedPosition;
    }

    void OnScroll(Vector2 pos)
    {
        float delta = Vector2.Distance(pos, lastPosition);
        
        if (delta >= scrollThreshold) 
        {
            AudioManager.Instance.PlaySfxUI(scrollSfxId, true);
            lastPosition = pos;
        }
    }

    void OnDestroy()
    {
        if (scrollRect != null)
            scrollRect.onValueChanged.RemoveListener(OnScroll);
    }
}
