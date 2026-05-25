using UnityEngine;
using System.Collections.Generic;

public class SelectorTeamEmblemSource : ISelectorSource<Emblem>
{
    public IEnumerable<Emblem> Enumerate() 
    {
        List<Emblem> list = new ();

        var dict = EmblemDatabase.Instance.Emblems;

        foreach (var kvp in dict)
            list.Add(kvp.Value);

        return list;
    }
}
