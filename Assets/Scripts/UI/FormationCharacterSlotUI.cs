using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Simulation.Enums.Character;

public class FormationCharacterSlotUI : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, ISelectHandler
{
    // TODO toggle between edit mode and view only mode
    // TODO on slot selected, show character info in the side panel

    [Header("UI Elements")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    // Runtime
    private int slotIndex;
    private FormationCoord coord;
    private Character character;
    private bool isBench;

    private Canvas rootCanvas;
    private Vector2 originalPosition;

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    // ============================================================
    //  INITIALIZATION
    // ============================================================

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

    public void UpdateCoord(FormationCoord newCoord)
    {
        coord = newCoord;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;

        if (character == null)
        {
            //hide card
            return;
        }
            //show card

        characterCard.SetCharacter(character, coord.Position);
    }

    public Character GetCharacter() => character;
    public int SlotIndex => slotIndex;
    public bool IsBench => isBench;

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

    // ============================================================
    //  DRAG & DROP (swap players between slots)
    // ============================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        FormationCharacterSlotUI draggedSlot = eventData.pointerDrag?.GetComponent<FormationCharacterSlotUI>();
        if (draggedSlot != null && draggedSlot != this)
        {
            SwapCharacters(draggedSlot);
        }
    }

    private void SwapCharacters(FormationCharacterSlotUI other)
    {
        UIEvents.RaiseFormationCharacterSlotUISwaped(this, other);
    }

    #region Events

    private void OnEnable()
    {
        UIEvents.OnFormationCharacterSlotUIReplaced += HandleFormationCharacterSlotUIReplaced;
    }

    private void OnDisable()
    {
        UIEvents.OnFormationCharacterSlotUIReplaced -= HandleFormationCharacterSlotUIReplaced;
    }

    private void HandleFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character) 
    {
        if(this != slot) return;
        SetCharacter(character);
    }

    #endregion
}
