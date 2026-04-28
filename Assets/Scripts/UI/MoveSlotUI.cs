using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class MoveSlotUI : MonoBehaviour
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

    private Move move;
    private Character character;
    private IconManager iconManager;

    public Move Move => move;
    public Character Character => character;
    public Button Button => button;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        imageBlock.enabled = true;
    }

    private void Start()
    {
        iconManager = IconManager.Instance;
    }

    #endregion

    #region Initialize

    public void Initialize(Move move, Character character)
    {
        this.move = move;
        this.character = character;
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
        UIEvents.RaiseMoveActionsOpenRequested(move, character);
    }

    #endregion

    #region Events

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    #endregion
}
