using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Aremoreno.Enums.Log;

public class ScrollViewAutoScroll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("Settings")]
    [SerializeField] private float scrollSpeed = 8f;
    [SerializeField] private float padding = 10f;
    [SerializeField] private bool instant = false;

    private RectTransform viewport;
    private RectTransform content;
    private GameObject lastSelected;

    private bool isScrolling;
    private Vector2 targetAnchoredPos;
    private bool isActive;

    // ─────────────────────────────────────────────
    private void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        if (scrollRect == null)
        {
            LogManager.Error("[AutoScroll] ScrollRect is NULL. Assign it in the Inspector.", this);
            return;
        }

        viewport = scrollRect.viewport;
        content  = scrollRect.content;

        if (viewport == null)
            LogManager.Error("[AutoScroll] Viewport is NULL. Check ScrollRect setup.", this);

        if (content == null)
            LogManager.Error("[AutoScroll] Content is NULL. Check ScrollRect setup.", this);

        LogManager.Debug($"[AutoScroll] Awake OK — viewport={viewport?.name} content={content?.name}", this);
    }

    // ─────────────────────────────────────────────
    public void Activate()
    {
        LogManager.Debug("[AutoScroll] Activate() called", this);
        isActive = true;
        StartCoroutine(DelayedReset());
    }

    private IEnumerator DelayedReset()
    {
        yield return null;
        lastSelected = null;
        LogManager.Debug("[AutoScroll] DelayedReset done — ready to evaluate selection", this);
    }

    public void Deactivate()
    {
        LogManager.Debug("[AutoScroll] Deactivate() called", this);
        isActive     = false;
        isScrolling  = false;
        lastSelected = null;
    }

    // ─────────────────────────────────────────────
    private void LateUpdate()
    {
        if (!isActive) return;

        GameObject selected = EventSystem.current?.currentSelectedGameObject;

        if (selected != lastSelected)
        {
            LogManager.Debug($"[AutoScroll] Selection changed: " +
                             $"{(lastSelected != null ? lastSelected.name : "NULL")} " +
                             $"→ {(selected != null ? selected.name : "NULL")}", this);
            lastSelected = selected;
            HandleSelectionChanged(selected);
        }

        if (!instant && isScrolling)
        {
            Vector2 next = Vector2.Lerp(
                content.anchoredPosition,
                targetAnchoredPos,
                Time.unscaledDeltaTime * scrollSpeed
            );

            if (Vector2.SqrMagnitude(next - targetAnchoredPos) < 0.1f)
            {
                content.anchoredPosition = targetAnchoredPos;
                isScrolling = false;
                LogManager.Debug($"[AutoScroll] Scroll complete. Final pos={content.anchoredPosition}", this);
            }
            else
            {
                content.anchoredPosition = next;
            }
        }
    }

    // ─────────────────────────────────────────────
    private void HandleSelectionChanged(GameObject selected)
    {
        if (selected == null)
        {
            LogManager.Debug("[AutoScroll] Selected is NULL — skipping", this);
            return;
        }

        if (!IsChildOfContent(selected.transform))
        {
            LogManager.Warning($"[AutoScroll] '{selected.name}' is NOT a child of content '{content?.name}' — skipping", this);
            return;
        }

        RectTransform item = selected.transform as RectTransform;
        if (item == null)
        {
            LogManager.Error($"[AutoScroll] '{selected.name}' has no RectTransform — skipping", this);
            return;
        }

        LogManager.Debug($"[AutoScroll] HandleSelectionChanged → scrolling to '{item.name}'", this);
        Canvas.ForceUpdateCanvases();
        ComputeAndApplyScroll(item);
    }

    // ─────────────────────────────────────────────
    private void ComputeAndApplyScroll(RectTransform item)
    {
        LogManager.Debug($"[AutoScroll] ── ComputeAndApplyScroll: {item.name}", this);
        LogManager.Debug($"[AutoScroll] content.anchoredPosition = {content.anchoredPosition}", this);
        LogManager.Debug($"[AutoScroll] content.rect  = {content.rect}", this);
        LogManager.Debug($"[AutoScroll] viewport.rect = {viewport.rect}", this);

        Vector2 itemCenterInViewport = viewport.InverseTransformPoint(
            item.TransformPoint(item.rect.center)
        );

        float itemHalfH = item.rect.height * 0.5f;
        float itemHalfW = item.rect.width  * 0.5f;
        float viewHalfH = viewport.rect.height * 0.5f;
        float viewHalfW = viewport.rect.width  * 0.5f;

        LogManager.Debug($"[AutoScroll] itemCenterInViewport = {itemCenterInViewport}", this);
        LogManager.Debug($"[AutoScroll] item  half W={itemHalfW} H={itemHalfH}", this);
        LogManager.Debug($"[AutoScroll] view  half W={viewHalfW} H={viewHalfH}", this);

        Vector2 newPos = content.anchoredPosition;
        bool changed   = false;

        // ── Vertical ───────────────────────────────
        if (scrollRect.vertical)
        {
            float itemTop    = itemCenterInViewport.y + itemHalfH;
            float itemBottom = itemCenterInViewport.y - itemHalfH;
            float viewTop    = viewport.rect.yMax - padding;
            float viewBottom = viewport.rect.yMin + padding;

            LogManager.Debug($"[AutoScroll] VERTICAL " +
                             $"itemTop={itemTop:F1} itemBottom={itemBottom:F1} " +
                             $"viewTop={viewTop:F1} viewBottom={viewBottom:F1}", this);

            if (itemBottom < viewBottom)
            {
                float delta = viewBottom - itemBottom;
                LogManager.Debug($"[AutoScroll] Item BELOW viewport → newPos.y += {delta:F1}", this);
                newPos.y += delta;
                changed = true;
            }
            else if (itemTop > viewTop)
            {
                float delta = itemTop - viewTop;
                LogManager.Debug($"[AutoScroll] Item ABOVE viewport → newPos.y -= {delta:F1}", this);
                newPos.y -= delta;
                changed = true;
            }
            else
            {
                LogManager.Debug("[AutoScroll] Item already visible vertically", this);
            }
        }

        // ── Horizontal ─────────────────────────────
        if (scrollRect.horizontal)
        {
            float itemRight = itemCenterInViewport.x + itemHalfW;
            float itemLeft  = itemCenterInViewport.x - itemHalfW;
            float viewRight = viewport.rect.xMax - padding;
            float viewLeft  = viewport.rect.xMin + padding;

            LogManager.Debug($"[AutoScroll] HORIZONTAL " +
                             $"itemLeft={itemLeft:F1} itemRight={itemRight:F1} " +
                             $"viewLeft={viewLeft:F1} viewRight={viewRight:F1}", this);

            if (itemLeft < viewLeft)
            {
                float delta = viewLeft - itemLeft;
                LogManager.Debug($"[AutoScroll] Item LEFT of viewport → newPos.x -= {delta:F1}", this);
                newPos.x -= delta;
                changed = true;
            }
            else if (itemRight > viewRight)
            {
                float delta = itemRight - viewRight;
                LogManager.Debug($"[AutoScroll] Item RIGHT of viewport → newPos.x += {delta:F1}", this);
                newPos.x += delta;
                changed = true;
            }
            else
            {
                LogManager.Debug("[AutoScroll] Item already visible horizontally", this);
            }
        }

        if (!changed)
        {
            LogManager.Debug("[AutoScroll] No scroll needed — item fully visible", this);
            return;
        }

        Vector2 clamped = ClampToContent(newPos);
        LogManager.Debug($"[AutoScroll] Pre-clamp={newPos} Post-clamp={clamped}", this);

        ApplyScroll(clamped);
    }

    // ─────────────────────────────────────────────
    private Vector2 ClampToContent(Vector2 pos)
    {
        float contentH  = content.rect.height;
        float contentW  = content.rect.width;
        float viewportH = viewport.rect.height;
        float viewportW = viewport.rect.width;

        if (scrollRect.vertical)
        {
            float maxY = Mathf.Max(0f, contentH - viewportH);
            pos.y = Mathf.Clamp(pos.y, 0f, maxY);
        }

        if (scrollRect.horizontal)
        {
            float maxX = Mathf.Max(0f, contentW - viewportW);
            pos.x = Mathf.Clamp(pos.x, -maxX, 0f);
        }

        return pos;
    }

    // ─────────────────────────────────────────────
    private void ApplyScroll(Vector2 pos)
    {
        if (instant)
        {
            LogManager.Debug($"[AutoScroll] ApplyScroll INSTANT → {pos}", this);
            content.anchoredPosition = pos;
            isScrolling = false;
        }
        else
        {
            LogManager.Debug($"[AutoScroll] ApplyScroll SMOOTH → target={pos}", this);
            targetAnchoredPos = pos;
            isScrolling       = true;
        }
    }

    // ─────────────────────────────────────────────
    private bool IsChildOfContent(Transform t)
    {
        const int maxDepth = 8;
        Transform current  = t;

        for (int i = 0; i < maxDepth; i++)
        {
            if (current == null)    return false;
            if (current == content) return true;
            current = current.parent;
        }
        return false;
    }

    // ─────────────────────────────────────────────
    public void ScrollTo(RectTransform target, bool forceInstant = false)
    {
        if (target == null)
        {
            LogManager.Error("[AutoScroll] ScrollTo called with NULL target", this);
            return;
        }

        LogManager.Debug($"[AutoScroll] ScrollTo called → {target.name} forceInstant={forceInstant}", this);

        bool wasInstant = instant;
        if (forceInstant) instant = true;

        Canvas.ForceUpdateCanvases();
        ComputeAndApplyScroll(target);

        instant = wasInstant;
    }

    public void ResetToTop()
    {
        LogManager.Debug("[AutoScroll] ResetToTop called", this);
        content.anchoredPosition = Vector2.zero;
        isScrolling  = false;
        lastSelected = null;
    }
}
