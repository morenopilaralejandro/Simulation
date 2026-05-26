using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class SelectorMoveListItem : SelectorListItem<Move>
{
    [Header("UI Elements")]
    [SerializeField] private Image imageBlock;
    [SerializeField] private MoveUI moveUI;

    protected override void OnBind(Move move)
    {
        IconManager iconManager = IconManager.Instance;

        moveUI.SetMoveAsync(move);
        imageBlock.enabled = false;
    }

    protected override void OnUnbind()
    {
        imageBlock.enabled = true;
        moveUI.Clear();
    }
}
