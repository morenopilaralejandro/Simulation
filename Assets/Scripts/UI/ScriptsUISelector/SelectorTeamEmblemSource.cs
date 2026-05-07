using UnityEngine;
using System.Collections.Generic;

public class SelectorTeamEmblemSource : ISelectorSource<SelectorTeamEmblemData>
{
    public IEnumerable<SelectorTeamEmblemData> Enumerate() 
    {
        List<SelectorTeamEmblemData> list = new ();

        var dict = SpriteAtlasManager.Instance.GetAllSpritesFromAtlasTeamCrest();

        foreach (var kvp in dict)
            list.Add(new SelectorTeamEmblemData(kvp.Key, kvp.Value));

        return list;
    }
}
