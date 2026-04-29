using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Aremoreno.Enums.Move;

public class SelectorMoveListItem : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [SerializeField] private Image imageBlock;
    [SerializeField] private Image imageCategoty;
    [SerializeField] private Image imageTrait;
    [SerializeField] private Image imageEvolution;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textCost;
    [SerializeField] private Button button;

    private Move move;
    private IconManager iconManager;

    public Button Button => button;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        iconManager = IconManager.Instance;
    }

    #endregion

    #region Initialize

    public void Initialize(Move move)
    {
        imageBlock.enabled = true;
        imageCategoty.enabled = false;
        imageTrait.enabled = false;
        imageEvolution.enabled = false;
        textName.text = "";
        textCost.text = "";

        this.move = move;

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

    #endregion

    #region Button

    public void OnListItemClicked() 
    {
        UIEvents.RaiseMoveSelected(move);
    }

    #endregion

    #region Events
    
    /*

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

    */

    #endregion
}
