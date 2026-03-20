using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Item;
using Simulation.Enums.Kit;

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
