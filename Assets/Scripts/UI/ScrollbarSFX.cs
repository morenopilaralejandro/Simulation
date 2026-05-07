using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarSFX : MonoBehaviour
{
    [Tooltip("Minimum value change before playing sound")]
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float threshold = 0.02f;

    private string scrollSfxId = "sfx-menu_scroll";
    private float lastValue;

    void Awake()
    {
        scrollbar.onValueChanged.AddListener(OnScrollbarChanged);
        lastValue = scrollbar.value;
    }

    void OnScrollbarChanged(float value)
    {
        if (Mathf.Abs(value - lastValue) >= threshold)
        {
            AudioManager.Instance.PlaySfxUI(scrollSfxId, true);
            lastValue = value;
        }
    }

    void OnDestroy()
    {
        if (scrollbar != null)
            scrollbar.onValueChanged.RemoveListener(OnScrollbarChanged);
    }
}
