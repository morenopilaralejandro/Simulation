using UnityEngine;

public class SelectorScoutTierAction : ISelectorClickAction<ScoutTier>
{
    public void Execute(ScoutTier obj, IClosableMenu menu)
    {
        UIEvents.RaiseScoutEntrySelectorOpenRequested(
            new SelectorScoutEntrySourceFromTier(obj),
            new SelectorScoutEntryAction(),
            null
        );
    }
}
