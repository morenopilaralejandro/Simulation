using UnityEngine;

public class SelectorWingActionEquipt : ISelectorClickAction<Wing>
{
    public void Execute(Wing obj, IClosableMenu menu)
    {
        UIEvents.RaiseSelectorWingActionClicked(obj);
    }
}
