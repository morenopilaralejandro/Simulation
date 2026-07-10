using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;

public class CharacterComponentEquipment
{
    #region Fields

    private Character character;

    private const int MaxEquipmentSlots = 4;
    private ItemEquipment[] equippedItems = new ItemEquipment[MaxEquipmentSlots];
    private Dictionary<Stat, int> equipmentStats = new();

    #endregion

    #region LifeCycle

    public CharacterComponentEquipment(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        this.character = character;

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            equipmentStats[stat] = 0;
        }

        if (characterSaveData == null) return;
    }

    #endregion

    #region Equip

    public void EquipEquipment(ItemEquipment itemEquipment)
    {
        equippedItems[(int)itemEquipment.EquipmentType] = itemEquipment;
        CalculateStats();
    }

    public void UnequipEquipment(ItemEquipment itemEquipment)
    {
        equippedItems[(int)itemEquipment.EquipmentType] = null;
        CalculateStats();
    }

    public ItemEquipment GetEquipment(int slot)
    {
        if (slot < 0 || slot >= MaxEquipmentSlots) return null;

        return equippedItems[slot];
    }
 
    #endregion

    #region Stats 

    private void CalculateStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            equipmentStats[stat] = 0;
        }

        foreach (var item in equippedItems)
        {
            if (item == null) continue;

            foreach (var stat in item.EquipmentStats)
            {
                equipmentStats[stat.Key] += stat.Value;
            }
        }
    }

    public int GetEquipmentStat(Stat stat) => equipmentStats[stat];

    #endregion

    #region Persistence

    #endregion

}
