using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class DPadController : OnScreenControl, 
    IPointerDownHandler, IPointerUpHandler, IDragHandler  // ← Added IDragHandler
{
    [InputControl(layout = "Vector2")]
    [SerializeField] private string m_ControlPath;

    [Header("Button References")]
    [SerializeField] private RectTransform btnUp;
    [SerializeField] private RectTransform btnDown;
    [SerializeField] private RectTransform btnLeft;
    [SerializeField] private RectTransform btnRight;

    [Header("Visual Feedback")]
    [SerializeField] private Image imgUp;
    [SerializeField] private Image imgDown;
    [SerializeField] private Image imgLeft;
    [SerializeField] private Image imgRight;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = new Color(1f, 1f, 1f, 0.5f);

    private Dictionary<int, RectTransform> pointerToButton = new Dictionary<int, RectTransform>();

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdatePointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)  // ← Fires every frame while finger moves
    {
        UpdatePointer(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerToButton.Remove(eventData.pointerId);
        UpdateDirectionAndVisuals();
    }

    private void UpdatePointer(PointerEventData eventData)
    {
        RectTransform hitBtn = GetHitButton(eventData);

        if (hitBtn != null)
            pointerToButton[eventData.pointerId] = hitBtn;
        else
            pointerToButton.Remove(eventData.pointerId);

        UpdateDirectionAndVisuals();
    }

    private RectTransform GetHitButton(PointerEventData eventData)
    {
        Camera cam = eventData.pressEventCamera;
        Vector2 pos = eventData.position;

        if (RectTransformUtility.RectangleContainsScreenPoint(btnUp, pos, cam))
            return btnUp;
        if (RectTransformUtility.RectangleContainsScreenPoint(btnDown, pos, cam))
            return btnDown;
        if (RectTransformUtility.RectangleContainsScreenPoint(btnLeft, pos, cam))
            return btnLeft;
        if (RectTransformUtility.RectangleContainsScreenPoint(btnRight, pos, cam))
            return btnRight;

        return null;
    }

    private void UpdateDirectionAndVisuals()
    {
        HashSet<RectTransform> activeButtons = new HashSet<RectTransform>(pointerToButton.Values);

        // --- Direction ---
        Vector2 dir = Vector2.zero;
        if (activeButtons.Contains(btnUp))    dir.y += 1;
        if (activeButtons.Contains(btnDown))  dir.y -= 1;
        if (activeButtons.Contains(btnLeft))  dir.x -= 1;
        if (activeButtons.Contains(btnRight)) dir.x += 1;

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        SendValueToControl(dir);

        // --- Visuals ---
        SetButtonColor(imgUp,    activeButtons.Contains(btnUp));
        SetButtonColor(imgDown,  activeButtons.Contains(btnDown));
        SetButtonColor(imgLeft,  activeButtons.Contains(btnLeft));
        SetButtonColor(imgRight, activeButtons.Contains(btnRight));
    }

    private void SetButtonColor(Image img, bool isPressed)
    {
        if (img != null)
            img.color = isPressed ? pressedColor : normalColor;
    }
}
