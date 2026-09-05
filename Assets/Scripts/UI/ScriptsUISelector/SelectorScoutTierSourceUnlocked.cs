using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectorScoutTierSourceUnlocked : ISelectorSource<ScoutTier>
{
    public IEnumerable<ScoutTier> Enumerate()
    {
        foreach (var tierData in DatabaseManager.Instance.DatabaseRegistry.ScoutTierData.Data.Values
                     .OrderBy(x => x.ScoutTierId))
        {
            if (StorySystemManager.Instance.HasFlag(tierData.UnlockFlag))
                yield return new ScoutTier(tierData);
        }
    }
}
