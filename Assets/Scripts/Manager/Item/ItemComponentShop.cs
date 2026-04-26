using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentShop
{
    private Item item;

    private Dictionary<CurrencyType, int> priceBuyDict = new();
    private int priceSellGold;

    public bool IsSellable { get; private set; }

    public ItemComponentShop(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        Initialize(itemData, item, itemSaveData);
    }

    public void Initialize(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        IsSellable = itemData.IsSellable;
        priceSellGold = itemData.PriceSellGold;

        priceBuyDict[CurrencyType.Gold] = itemData.PriceBuyGold;
    }

    public int GetPriceBuy(CurrencyType currencyType = CurrencyType.Gold) 
    {
        return priceBuyDict[currencyType];
    }

    public int GetPriceSell() 
    {
        return IsSellable ? priceSellGold : 0;
    }
}
