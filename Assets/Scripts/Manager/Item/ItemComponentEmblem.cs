using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentEmblem
{
    private Item item;
    public string EmblemId { get; private set; }

    public ItemComponentEmblem(ItemDataEmblem itemDataEmblem, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        EmblemId = itemDataEmblem.EmblemId;
    }
}
