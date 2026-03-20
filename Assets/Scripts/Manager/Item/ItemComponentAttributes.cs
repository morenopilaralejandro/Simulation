using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Item;

public class ItemComponentAttributes
{
    public string ItemId { get; private set; }
    public ItemCategory Category { get; private set; }

    public ItemComponentAttributes(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        Initialize(itemData, item, itemSaveData);
    }

    public void Initialize(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        ItemId = itemData.ItemId;
        Category = itemData.Category;
    }
}
