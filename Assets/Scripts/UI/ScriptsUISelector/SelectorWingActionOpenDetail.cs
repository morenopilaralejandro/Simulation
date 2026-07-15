using UnityEngine;

public class SelectorWingActionOpenDetail : ISelectorClickAction<Wing>
{
    public void Execute(Wing obj, IClosableMenu menu)
    {
        UIEvents.RaiseWingDetailOpenRequested(obj);
    }
}
