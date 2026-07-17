using UnityEngine;
using System.Collections.Generic;

public class SelectorWingSourceFromStorage : ISelectorSource<Wing>
{
    public IEnumerable<Wing> Enumerate()
        => WingManager.Instance.Wings.Values;
}
