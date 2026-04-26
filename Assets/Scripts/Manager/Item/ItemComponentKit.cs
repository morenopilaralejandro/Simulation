using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Kit;

public class ItemComponentKit
{
    private Item item;
    public string KitId { get; private set; }

    public ItemComponentKit(ItemDataKit itemDataKit, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        KitId = itemDataKit.KitId;
    }
}
