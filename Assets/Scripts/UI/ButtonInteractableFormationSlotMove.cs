using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractableFormationSlotMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _targetButton;

    private void OnEnable()
    {
        UIEvents.OnFormationCharacterSlotUIMoveStarted += HandleMoveStarted;
        UIEvents.OnFormationCharacterSlotUIMoveEnded += HandleMoveEnded;
    }

    private void OnDisable()
    {
        UIEvents.OnFormationCharacterSlotUIMoveStarted -= HandleMoveStarted;
        UIEvents.OnFormationCharacterSlotUIMoveEnded -= HandleMoveEnded;
    }

    private void HandleMoveStarted(FormationCharacterSlotUI slot)
    {
        _targetButton.interactable = false;
    }

    private void HandleMoveEnded(FormationCharacterSlotUI slot)
    {
        _targetButton.interactable = true;
    }
}
