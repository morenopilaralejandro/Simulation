using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectorFastTravelPointSourceUnlocked : ISelectorSource<FastTravelPoint>
{
    public IEnumerable<FastTravelPoint> Enumerate()
    {
        foreach (var data in DatabaseManager.Instance.DatabaseRegistry.FastTravelPointData.Data.Values
            .OrderBy(x => x.FastTravelPointId))
        {
            if (StorySystemManager.Instance.HasFlag(data.FlagId))
                yield return new FastTravelPoint(data);
        }
    }
}
