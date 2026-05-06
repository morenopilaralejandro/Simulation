using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TMPInputFieldNoAutoActivate : TMP_InputField
{
    public override void OnSelect(BaseEventData eventData)
    {
        // Let TMP do its full OnSelect (handles visuals properly)
        base.OnSelect(eventData);

        // Then immediately deactivate the keyboard/edit mode
        DeactivateInputField();
    }

    public void ActivateManually()
    {
        ActivateInputField();
    }
}
