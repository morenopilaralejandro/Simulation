using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Wing;

public class ItemComponentWing
{
    private Item item;
    public string WingId { get; private set; }

    public ItemComponentWing(ItemDataWing itemDataWing, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        WingId = itemDataWing.WingId;
    }
}
