using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Item;

public class SelectorEquipmentSourceFromStorageByType : ISelectorSource<ItemEquipment>
{
    private EquipmentType equipmentType;
    public EquipmentType EquipmentType => equipmentType;

    public SelectorEquipmentSourceFromStorageByType() {}
    public SelectorEquipmentSourceFromStorageByType(EquipmentType equipmentType) 
    {
        this.equipmentType = equipmentType;
    }

    public IEnumerable<ItemEquipment> Enumerate()
    {
        List<ItemStorageSlot> slots =
            ItemManager.Instance.GetItemsByCategory(ItemCategory.Equipment);

        for (int i = 0; i < slots.Count; i++)
        {
            ItemEquipment equipment = slots[i].Item as ItemEquipment;
            if (equipment != null && equipment.EquipmentType == equipmentType)
            {
                yield return equipment;
            }
        }
    }
}
