using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;

public class ItemComponentEquipment
{
    private Item item;
    private Dictionary<Stat, int> equipmentStats = new();

    public EquipmentType EquipmentType { get; private set; }
    public IReadOnlyDictionary<Stat, int> EquipmentStats => equipmentStats;

    public ItemComponentEquipment(ItemDataEquipment itemDataEquipment, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        EquipmentType = itemDataEquipment.EquipmentType;
        PopulateDict(itemDataEquipment);
    }

    private void PopulateDict(ItemDataEquipment itemDataEquipment) 
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            int statValue = GetBaseStatValueFromData(stat, itemDataEquipment);
            equipmentStats[stat] = statValue;
        }
    }

    private int GetBaseStatValueFromData(Stat stat, ItemDataEquipment itemDataEquipment)
    {
        return stat switch
        {
            Stat.Hp => itemDataEquipment.EquipmentHp,
            Stat.Sp => itemDataEquipment.EquipmentSp,
            Stat.Kick => itemDataEquipment.EquipmentKick,
            Stat.Body => itemDataEquipment.EquipmentBody,
            Stat.Control => itemDataEquipment.EquipmentControl,
            Stat.Guard => itemDataEquipment.EquipmentGuard,
            Stat.Speed => itemDataEquipment.EquipmentSpeed,
            Stat.Stamina => itemDataEquipment.EquipmentStamina,
            Stat.Technique => itemDataEquipment.EquipmentTechnique,
            Stat.Luck => itemDataEquipment.EquipmentLuck,
            Stat.Courage => itemDataEquipment.EquipmentCourage,
            _ => 0
        };
    }

    /*
    TODO 
    Sum all equip on character
    Add it on get battle stat

    private Dictionary<Stat, int> totalStats = new Dictionary<Stat, int>();

    void CalculateTotalStats()
    {
        totalStats.Clear(); // reuse, no GC allocation

        for (int i = 0; i < objects.Count; i++) // avoid enumerator allocation
        {
            var dict = objects[i].battleStats;
            foreach (var kvp in dict)
            {
                totalStats.TryGetValue(kvp.Key, out int current);
                totalStats[kvp.Key] = current + kvp.Value;
            }
        }
    }

    */
}
