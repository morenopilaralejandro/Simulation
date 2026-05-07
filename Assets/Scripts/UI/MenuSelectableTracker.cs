using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class MenuSelectableTracker : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        UIEvents.RaiseSelectableSelected(gameObject);
    }
}
