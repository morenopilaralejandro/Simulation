using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractableFormationSlotMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _targetButton;

    private bool _wasInteractableBeforeMove;

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
        _wasInteractableBeforeMove = _targetButton.interactable;
        _targetButton.interactable = false;
    }

    private void HandleMoveEnded(FormationCharacterSlotUI slot)
    {
        if (_wasInteractableBeforeMove)
            _targetButton.interactable = true;
    }
}
