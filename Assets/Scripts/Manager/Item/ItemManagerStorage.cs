using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Item;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

public class ItemManagerStorage
{
    #region Fields

    private List<ItemStorageSlot> listEquipment = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listFormation = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listImportant = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listKit = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listMaterial = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listMisc = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listMove = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listRecovery = new List<ItemStorageSlot>();

    private Dictionary<ItemCategory, List<ItemStorageSlot>> categoryMap;
    private ItemStorageSlot cachedSlot;

    #endregion

    #region Constructor

    public ItemManagerStorage() 
    {
        categoryMap = new Dictionary<ItemCategory, List<ItemStorageSlot>>
        {
            { ItemCategory.Equipment, listEquipment },
            { ItemCategory.Formation, listFormation },
            { ItemCategory.Important, listImportant },
            { ItemCategory.Kit, listKit },
            { ItemCategory.Material, listMaterial },
            { ItemCategory.Misc, listMisc },
            { ItemCategory.Move, listMove },
            { ItemCategory.Recovery, listRecovery }
        };
     }

    #endregion

    #region First Time Initialize

    public void FirstTimeInitialize()
    {
        AddItem(ItemFactory.CreateById("spike_cool"), 10);
    }

    #endregion

    #region Add / Remove
    public void AddItem(Item item, int count = 1)
    {
        SetSlot(item);

        if (cachedSlot != null)
            cachedSlot.AddCount(count);
        else
            categoryMap[item.Category].Add(new ItemStorageSlot(item, count));

        ItemEvents.RaiseStorageUpdated();
    }

    public bool RemoveItem(Item item, int count = 1)
    {
        SetSlot(item);

        if (cachedSlot == null) return false;

        bool success = cachedSlot.RemoveCount(count);
        if (success && cachedSlot.Count <= 0)
            categoryMap[item.Category].Remove(cachedSlot);

        ItemEvents.RaiseStorageUpdated();
        return success;
    }

    /*
    public bool UseItem(Item item, Character target)
    {
        bool success = item.Use(target);
        
        if (success && item.isConsumable)
        {
            RemoveItem(item);
        }

        return success;
    }
    */

    #endregion

    #region Helpers

    private void SetSlot(Item item) 
    {
        cachedSlot = null;
        for (int i = 0; i < categoryMap[item.Category].Count; i++)
        {
            if (categoryMap[item.Category][i].Item.ItemId == item.ItemId)
            {
                cachedSlot = categoryMap[item.Category][i];
                break;
            }
        }
    }

    #endregion

    #region Query

    public bool HasItem(Item item)
    {
        SetSlot(item);
        return cachedSlot != null && cachedSlot.Count > 0;
    }

    public int GetItemCount(Item item)
    {
        SetSlot(item);
        return cachedSlot?.Count ?? 0;
    }

    #endregion

    #region Persistence
    
    public ItemStorageSaveData Export()
    {
        ItemStorageSaveData saveData = new ItemStorageSaveData();
        saveData.ItemStorageSlotSaveDataList = new List<ItemStorageSlotSaveData>();

        foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
        {
            foreach (ItemStorageSlot slot in categoryMap[category])
            {
                saveData.ItemStorageSlotSaveDataList.Add(
                    new ItemStorageSlotSaveData {
                        ItemId = slot.Item.ItemId,
                        Category = slot.Item.Category,
                        Count = slot.Count
                    }         
                );
            }
        }
        return saveData;
    }

    public void Import(ItemStorageSaveData saveData)
    {
        //clear

        if (saveData?.ItemStorageSlotSaveDataList == null) return;

        foreach (ItemStorageSlotSaveData itemStorageSlotSaveData in saveData.ItemStorageSlotSaveDataList)
        {
            AddItem(
                ItemFactory.CreateByIdAndCategory(itemStorageSlotSaveData.ItemId, itemStorageSlotSaveData.Category), 
                itemStorageSlotSaveData.Count
            );
        }
    }

    #endregion

}
