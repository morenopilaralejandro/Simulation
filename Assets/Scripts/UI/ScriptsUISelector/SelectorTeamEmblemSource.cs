using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Aremoreno.Enums.Item;

public class SelectorTeamEmblemSource : ISelectorSource<Emblem>
{
    public IEnumerable<Emblem> Enumerate()
    {
        var slots = ItemManager.Instance.GetItemsByCategory(ItemCategory.Emblem);

        return slots
            .Select(slot => (ItemEmblem)slot.Item)
            .Select(item => new Emblem(DatabaseManager.Instance.GetEmblemData(item.EmblemId)))
            .OrderBy(emblem => emblem.EmblemId);
    }
}
