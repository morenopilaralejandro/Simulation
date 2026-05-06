using UnityEngine;
using UnityEngine.EventSystems;

public class ActivateInputOnSubmit : MonoBehaviour, ISubmitHandler
{
    [SerializeField] private TMPInputFieldNoAutoActivate input;

    public void OnSubmit(BaseEventData eventData)
    {
        if (!input.isFocused)
            input.ActivateManually();
    }
}
