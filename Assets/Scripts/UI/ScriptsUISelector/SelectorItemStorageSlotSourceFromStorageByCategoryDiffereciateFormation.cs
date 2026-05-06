using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Item;

public class SelectorItemStorageSlotSourceFromStorageByCategoryDiffereciateFormation : ISelectorSource<ItemStorageSlot>
{
    private List<ItemStorageSlot> list = new();
    private ItemCategory category;
    private BattleType battleType;

    public SelectorItemStorageSlotSourceFromStorageByCategoryDiffereciateFormation() {}
    public SelectorItemStorageSlotSourceFromStorageByCategoryDiffereciateFormation(
        ItemCategory category,
        BattleType battleType) 
    {
        this.category = category;
        this.battleType = battleType;
    }

    public void SetCategory(ItemCategory category) 
    {
        this.category = category;
    }

    public void SetBattleType(BattleType battleType) 
    {
        this.battleType = battleType;
    }

    public IEnumerable<ItemStorageSlot> Enumerate() 
    {
        list.Clear();

        var source = ItemManager.Instance.GetItemsByCategory(category);

        foreach (var slot in source)
        {
            if (slot.Item.Category == ItemCategory.Formation && 
                !ItemManager.Instance.IsFormationOfBattleType(slot.Item, battleType)) 
                continue;

            list.Add(slot);
        }

        return list;
    }
}
