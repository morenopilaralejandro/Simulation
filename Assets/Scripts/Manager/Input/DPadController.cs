using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class DPadController : OnScreenControl,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
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

    [Tooltip("Tint when not pressed (full white = no tint). Alpha=1 always; CanvasGroup handles overall opacity.")]
    [SerializeField] private Color normalTint = Color.white;

    [Tooltip("Tint when pressed. Use a darker/lighter shade. Alpha=1; CanvasGroup handles overall opacity.")]
    [SerializeField] private Color pressedTint = new Color(0.7f, 0.7f, 0.7f, 1f);

    private readonly Dictionary<int, RectTransform> pointerToButton = new();

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public void OnPointerDown(PointerEventData eventData) => UpdatePointer(eventData);
    public void OnDrag(PointerEventData eventData) => UpdatePointer(eventData);

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

        if (RectTransformUtility.RectangleContainsScreenPoint(btnUp, pos, cam)) return btnUp;
        if (RectTransformUtility.RectangleContainsScreenPoint(btnDown, pos, cam)) return btnDown;
        if (RectTransformUtility.RectangleContainsScreenPoint(btnLeft, pos, cam)) return btnLeft;
        if (RectTransformUtility.RectangleContainsScreenPoint(btnRight, pos, cam)) return btnRight;

        return null;
    }

    private void UpdateDirectionAndVisuals()
    {
        HashSet<RectTransform> active = new(pointerToButton.Values);

        Vector2 dir = Vector2.zero;
        if (active.Contains(btnUp))    dir.y += 1;
        if (active.Contains(btnDown))  dir.y -= 1;
        if (active.Contains(btnLeft))  dir.x -= 1;
        if (active.Contains(btnRight)) dir.x += 1;

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        SendValueToControl(dir);

        // Visuals: tint only, alpha stays at 1 — CanvasGroup controls transparency
        SetTint(imgUp,    active.Contains(btnUp));
        SetTint(imgDown,  active.Contains(btnDown));
        SetTint(imgLeft,  active.Contains(btnLeft));
        SetTint(imgRight, active.Contains(btnRight));
    }

    private void SetTint(Image img, bool pressed)
    {
        if (img != null)
            img.color = pressed ? pressedTint : normalTint;
    }
}
