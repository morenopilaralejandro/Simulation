using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Item;

public class SelectorTeamEmblemSource : ISelectorSource<Emblem>
{
    public IEnumerable<Emblem> Enumerate()
    {
        var slots = ItemManager.Instance.GetItemsByCategory(ItemCategory.Emblem);

        foreach (var slot in slots)
        {
            var itemEmblem = (ItemEmblem)slot.Item;
            yield return new Emblem(DatabaseManager.Instance.GetEmblemData(itemEmblem.EmblemId));
        }
    }

    /*
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
    */
}
