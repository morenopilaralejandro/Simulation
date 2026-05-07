using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectForwarder : MonoBehaviour,
    IScrollHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IInitializePotentialDragHandler
{
    private ScrollRect sr;

    public void SetScrollRect(ScrollRect scrollRect) => sr = scrollRect;

    public void OnScroll(PointerEventData e)                  { if (sr != null) sr.OnScroll(e); }
    public void OnInitializePotentialDrag(PointerEventData e) { if (sr != null) sr.OnInitializePotentialDrag(e); }
    public void OnBeginDrag(PointerEventData e)               { if (sr != null) sr.OnBeginDrag(e); }
    public void OnDrag(PointerEventData e)                    { if (sr != null) sr.OnDrag(e); }
    public void OnEndDrag(PointerEventData e)                 { if (sr != null) sr.OnEndDrag(e); }
}
