using UnityEngine;

public class SelectorMoveAction : ISelectorClickAction<Move>
{
    public void Execute(Move move, IClosableMenu menu)
    {
        UIEvents.RaiseSelectorMoveActionClicked(move);
    }
}
