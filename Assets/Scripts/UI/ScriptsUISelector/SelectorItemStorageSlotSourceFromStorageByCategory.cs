using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Item;

public class SelectorItemStorageSlotSourceFromStorageByCategory : ISelectorSource<ItemStorageSlot>
{
    private ItemCategory category;

    public SelectorItemStorageSlotSourceFromStorageByCategory() {}
    public SelectorItemStorageSlotSourceFromStorageByCategory(ItemCategory category) 
    {
        this.category = category;
    }

    public void SetCategory(ItemCategory category) 
    {
        this.category = category;
    }

    public IEnumerable<ItemStorageSlot> Enumerate() 
        => ItemManager.Instance.GetItemsByCategory(category);
}
