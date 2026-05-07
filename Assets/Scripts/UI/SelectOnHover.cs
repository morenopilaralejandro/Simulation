using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Selectable selectable;
    private static Vector2 lastMousePos;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectable.IsInteractable()) return;
        if (EventSystem.current == null) return;
        if (EventSystem.current.alreadySelecting) return;
        if (EventSystem.current.currentSelectedGameObject == gameObject) return;
        if (eventData.position == lastMousePos) return;
        lastMousePos = eventData.position;

        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
