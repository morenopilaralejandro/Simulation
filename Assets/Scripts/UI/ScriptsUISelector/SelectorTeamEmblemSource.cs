using UnityEngine;
using System.Collections.Generic;

public class SelectorTeamEmblemSource : ISelectorSource<Emblem>
{
    public IEnumerable<Emblem> Enumerate() 
    {
        List<Emblem> list = new ();

        var dict = DatabaseManager.Instance.DatabaseRegistry.EmblemData.Data;

        foreach (var kvp in dict) 
        {
            list.Add(new Emblem(kvp.Value));
        }
        
        return list;
    }
}
