using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class SelectorMoveListItem : SelectorListItem<Move>
{
    [Header("UI Elements")]
    [SerializeField] private Image imageBlock;
    [SerializeField] private Image imageCategoty;
    [SerializeField] private Image imageTrait;
    [SerializeField] private Image imageEvolution;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textCost;

    protected override void OnBind(Move move)
    {
        IconManager iconManager = IconManager.Instance;

        imageBlock.enabled = false;
        imageTrait.enabled = false;
        imageEvolution.enabled = false;

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

    protected override void OnUnbind()
    {
        imageBlock.enabled = true;
        imageCategoty.enabled = false;
        imageTrait.enabled = false;
        imageEvolution.enabled = false;
        textName.text = "";
        textCost.text = "";
    }
}
