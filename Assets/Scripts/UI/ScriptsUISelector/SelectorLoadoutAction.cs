using UnityEngine;

public class SelectorLoadoutAction : ISelectorClickAction<Team>
{
    public void Execute(Team obj, IClosableMenu menu)
    {
        UIEvents.RaiseSelectorLoadoutActionClicked(obj);
    }
}
