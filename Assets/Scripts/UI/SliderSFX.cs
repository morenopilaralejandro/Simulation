using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderSFX : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Slider slider;
    private string dragStartId = "sfx-menu_tap";
    private string dragEndId = "sfx-menu_tap";
    private string tickId = "sfx-menu_change";

    [Header("Tick Settings")]
    [SerializeField] private float valueStep = 0.05f;

    private float lastSoundValue;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);

        lastSoundValue = slider.value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfxUI(dragStartId);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfxUI(dragEndId);
    }

    private void OnSliderChanged(float value)
    {
        float delta =
            Mathf.Abs(value - lastSoundValue);

        if (delta >= valueStep)
        {
            lastSoundValue = value;

            AudioManager.Instance.PlaySfxUI(tickId);
        }
    }
}
