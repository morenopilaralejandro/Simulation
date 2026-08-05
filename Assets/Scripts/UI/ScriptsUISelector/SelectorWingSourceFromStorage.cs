using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectorWingSourceFromStorage : ISelectorSource<Wing>
{
    public IEnumerable<Wing> Enumerate()
        => WingManager.Instance.Wings.Values
            .OrderBy(w => w.WingId);
}
