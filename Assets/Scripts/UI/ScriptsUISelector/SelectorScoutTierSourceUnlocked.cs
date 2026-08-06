using UnityEngine;
using System.Collections.Generic;

public class SelectorScoutTierSourceUnlocked : ISelectorSource<ScoutTier>
{
    public IEnumerable<ScoutTier> Enumerate()
    {
        foreach (var tierData in DatabaseManager.Instance.DatabaseRegistry.ScoutTierData.Data.Values)
        {
            if (StorySystemManager.Instance.HasFlag(tierData.UnlockFlag))
                yield return new ScoutTier(tierData);
        }
    }
}
