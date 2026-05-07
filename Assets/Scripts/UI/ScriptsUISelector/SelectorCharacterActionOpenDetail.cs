using UnityEngine;

public class SelectorCharacterActionOpenDetail : ISelectorClickAction<Character>
{
    public void Execute(Character c, IClosableMenu menu)
    {
        UIEvents.RaiseCharacterDetailOpenRequested(c);
    }
}
