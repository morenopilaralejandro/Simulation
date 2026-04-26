using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentInventory
{
    private Item item;

    public ItemUsageContext UsageContext { get; private set; }
    public bool IsDiscardable { get; private set; }
    public bool IsStackable { get; private set; }
    public int MaxStack { get; private set; }

    public ItemComponentInventory(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        Initialize(itemData, item, itemSaveData);
    }

    public void Initialize(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        UsageContext = itemData.UsageContext;
        IsDiscardable = itemData.IsDiscardable;
        IsStackable = itemData.IsStackable;
        MaxStack = itemData.MaxStack;
    }
}
