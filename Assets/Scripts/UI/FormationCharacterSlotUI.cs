using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;

public class FormationCharacterSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, ISelectHandler
{
    #region Field

    [Header("UI Elements")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    [Header("Drag Layer")]
    [SerializeField] private RectTransform dragLayer;

    // Runtime
    private int slotIndex;
    private FormationCoord coord;
    private Character character;
    private bool isBench;

    private Canvas rootCanvas;
    private Vector2 originalPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;

    public int SlotIndex => slotIndex;
    public bool IsBench => isBench;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    #endregion

    #region Initialize

    public void Initialize(int index, FormationCoord formationCoord)
    {
        slotIndex = index;
        coord = formationCoord;
        isBench = false;
    }

    public void SetAsBench(int benchIndex)
    {
        slotIndex = benchIndex;
        isBench = true;
    }

    /// <summary>
    /// Call once after pooling or instantiation to assign the shared drag layer.
    /// </summary>
    public void SetDragLayer(RectTransform layer)
    {
        dragLayer = layer;
    }

    #endregion

    #region Helpers

    public void UpdateCoord(FormationCoord newCoord)
    {
        coord = newCoord;
        characterCard.SetCharacter(character, newCoord.Position);
    }

    public Character GetCharacter() => character;

    public void SetCharacter(Character character)
    {
        this.character = character;

        if (character == null)
        {
            // hide card
            return;
        }
        // show card

        if (isBench)
            characterCard.SetCharacter(character, character.Position);
        else
            characterCard.SetCharacter(character, coord.Position);
    }

    public void SetVisible(bool boolValue)
    {
        canvasGroup.alpha = boolValue ? 1f : 0f;
        canvasGroup.interactable = boolValue;
        canvasGroup.blocksRaycasts = boolValue;
    }

    public void Reset()
    {
        slotIndex = -1;
        coord = default;
        character = null;
        isBench = false;
    }

    #endregion

    #region Button Handle

    public void OnButtonFormationCharacterSlotUIClicked()
    {
        UIEvents.RaiseFormationCharacterSlotUIClicked(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIEvents.RaiseFormationCharacterSlotUIHighlited(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        UIEvents.RaiseTeamButtonSelected(this.gameObject);
    }

    #endregion

    #region DRAG & DROP

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIEvents.RaiseFormationCharacterSlotUIMoveCanceled(this);

        // Store everything we need to restore later
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;

        // Capture world position before reparenting
        Vector3 worldPos = rectTransform.position;

        // Reparent into the drag layer (renders on top of everything)
        transform.SetParent(dragLayer, false);

        // Normalize anchors/pivot so anchoredPosition math is clean
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Restore the exact world position after reparent
        rectTransform.position = worldPos;

        // Visual feedback
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Visual feedback restore
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Reparent back to original parent
        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);

        // Restore original anchors, pivot, and position
        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.pivot = originalPivot;
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        FormationCharacterSlotUI draggedSlot =
            eventData.pointerDrag?.GetComponent<FormationCharacterSlotUI>();

        if (draggedSlot != null && draggedSlot != this)
        {
            SwapCharacters(draggedSlot);
        }
    }

    private void SwapCharacters(FormationCharacterSlotUI other)
    {
        UIEvents.RaiseFormationCharacterSlotUISwaped(this, other);
    }

    #endregion

    #region Move

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnFormationCharacterSlotUIReplaced += HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIMoveStarted += HandleMoveStarted;
        UIEvents.OnFormationCharacterSlotUIMoveEnded += HandleMoveEnded;
    }

    private void OnDisable()
    {
        UIEvents.OnFormationCharacterSlotUIReplaced -= HandleFormationCharacterSlotUIReplaced;
        UIEvents.OnFormationCharacterSlotUIMoveStarted -= HandleMoveStarted;
        UIEvents.OnFormationCharacterSlotUIMoveEnded -= HandleMoveEnded;
    }

    private void HandleFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character)
    {
        if (this != slot) return;
        SetCharacter(character);
    }

    private void HandleMoveStarted(FormationCharacterSlotUI slot)
    {
        if (this != slot) return;
        canvasGroup.alpha = 0.6f;
    }

    private void HandleMoveEnded(FormationCharacterSlotUI slot)
    {
        if (canvasGroup.alpha == 0.6f)
            canvasGroup.alpha = 1f;
    }

    #endregion
}
