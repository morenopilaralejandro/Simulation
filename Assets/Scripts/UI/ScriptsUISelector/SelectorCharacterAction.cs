using UnityEngine;

public class SelectorCharacterAction : ISelectorClickAction<Character>
{
    public void Execute(Character c, IClosableMenu menu)
    {
        UIEvents.RaiseSelectorCharacterActionClicked(c);
    }
}
