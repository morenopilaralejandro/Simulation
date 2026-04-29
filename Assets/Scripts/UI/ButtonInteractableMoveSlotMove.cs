using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractableMoveSlotMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _targetButton;

    private bool _wasInteractableBeforeMove;

    private void OnEnable()
    {
        UIEvents.OnMoveSlotUIMoveStarted += HandleMoveStarted;
        UIEvents.OnMoveSlotUIMoveEnded += HandleMoveEnded;
    }

    private void OnDisable()
    {
        UIEvents.OnMoveSlotUIMoveStarted -= HandleMoveStarted;
        UIEvents.OnMoveSlotUIMoveEnded -= HandleMoveEnded;
    }

    private void HandleMoveStarted(MoveSlotUI slot)
    {
        _wasInteractableBeforeMove = _targetButton.interactable;
        _targetButton.interactable = false;
    }

    private void HandleMoveEnded(MoveSlotUI slot)
    {
        if (_wasInteractableBeforeMove)
            _targetButton.interactable = true;
    }
}
