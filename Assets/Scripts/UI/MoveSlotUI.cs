using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class MoveSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    #region Field

    [Header("UI Elements")]
    [SerializeField] private Image imageBlock;
    [SerializeField] private Image imageCategoty;
    [SerializeField] private Image imageTrait;
    [SerializeField] private Image imageEvolution;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textCost;
    [SerializeField] private Button button;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    private Move move;
    private Character character;
    private int index;
    private IconManager iconManager;

    public Move Move => move;
    public Character Character => character;
    public int Index => index;
    public Button Button => button;

    private bool canDrag;
    private bool isDragging;

    private RectTransform dragLayer;
    private Canvas rootCanvas;
    private Vector2 originalPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        imageBlock.enabled = true;

        rootCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        iconManager = IconManager.Instance;
    }

    #endregion

    #region Initialize

    public void Initialize(Move move, Character character, int index, RectTransform dragLayer, bool canDrag)
    {
        this.move = move;
        this.character = character;
        this.index = index;

        this.dragLayer = dragLayer;
        this.canDrag = canDrag;
    }

    #endregion

    #region Helpers

    public void SetMove(Move move)
    {
        this.move = move;
        if (move == null) return;

        imageBlock.enabled = false;

        textName.text = move.MoveName;
        textName.color = ColorManager.GetElementColor(move.Element);
        textCost.text = $"{move.Cost}";

        imageCategoty.sprite = iconManager.Category.GetIcon(move.Category);
        imageCategoty.enabled = true;

        if (move.Trait != Trait.None) 
        {
            imageTrait.sprite = iconManager.Trait.GetIcon(move.Trait);
            imageTrait.enabled = true;
        }

        if (move.CurrentEvolution != MoveEvolution.None) 
        {
            imageEvolution.sprite = move.EvolutionSprite;
            imageEvolution.enabled = true;
        }
    }

    public void Clear()
    {   
        character = null;
        move = null;
        index = -1;

        imageBlock.enabled = true;
        imageCategoty.enabled = false;
        imageTrait.enabled = false;
        imageEvolution.enabled = false;
        textName.text = "";
        textCost.text = "";
    }

    #endregion

    #region Button Handle

    public void OnClick()
    {
        UIEvents.RaiseMoveSlotUIClicked(this);
    }

    #endregion

    #region Events

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
        if (this != slot) return;
        canvasGroup.alpha = 0.6f;
    }

    private void HandleMoveEnded(MoveSlotUI slot)
    {
        if (canvasGroup.alpha == 0.6f)
            canvasGroup.alpha = 1f;
    }

    #endregion

    #region Helpers

    public void SetVisible(bool boolValue)
    {
        canvasGroup.alpha = boolValue ? 1f : 0f;
        canvasGroup.interactable = boolValue;
        canvasGroup.blocksRaycasts = boolValue;
    }

    #endregion

    #region DRAG & DROP

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIEvents.RaiseMoveSlotUIMoveCanceled(this);

        if (!canDrag)
        {
            isDragging = false;
            return;
        }

        isDragging = true;

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
        if (!isDragging) return;
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return; // ← Guard
        isDragging = false;

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
        MoveSlotUI draggedSlot =
            eventData.pointerDrag?.GetComponent<MoveSlotUI>();

        if (draggedSlot != null && draggedSlot != this && draggedSlot.isDragging)
            SwapMoves(draggedSlot);
    }

    private void SwapMoves(MoveSlotUI other)
    {
        if (this.Character == null || other.Character == null) return;
        UIEvents.RaiseMoveSwapRequested(other.character, this.index, other.index);
    }

    #endregion
}
