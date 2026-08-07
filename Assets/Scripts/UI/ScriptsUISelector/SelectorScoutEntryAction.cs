using UnityEngine;

public class SelectorScoutEntryAction : ISelectorClickAction<ScoutEntry>
{
    public void Execute(ScoutEntry obj, IClosableMenu menu)
    {
        UIEvents.RaiseSelectorScoutEntryActionClicked(obj);
    }
}
