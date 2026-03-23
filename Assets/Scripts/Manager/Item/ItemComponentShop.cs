using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Item;

public class ItemComponentShop
{
    private Item item;

    public bool IsSellable  { get; private set; }
    public int PriceBuy     { get; private set; }
    public int PriceSell    { get; private set; }

    public ItemComponentShop(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        Initialize(itemData, item, itemSaveData);
    }

    public void Initialize(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        IsSellable = itemData.IsSellable;
        PriceBuy = itemData.PriceBuy;
        PriceSell = itemData.PriceSell;
    }
}
