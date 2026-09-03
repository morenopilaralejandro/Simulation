using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Localization;

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
    private List<ItemStorageSlot> listRecipe = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listEmblem = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listCharacter = new List<ItemStorageSlot>();
    private List<ItemStorageSlot> listWing = new List<ItemStorageSlot>();

    private Dictionary<ItemCategory, List<ItemStorageSlot>> categoryMap;
    private ItemStorageSlot cachedSlot;

    private ItemFormation cachedItemFormation;
    private Formation cachedFormation;

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
            { ItemCategory.Recovery, listRecovery },
            { ItemCategory.Recipe, listRecipe },
            { ItemCategory.Emblem, listEmblem },
            { ItemCategory.Character, listCharacter },
            { ItemCategory.Wing, listWing }
        };
     }

    #endregion

    #region First Time Initialize

    public void FirstTimeInitialize()
    {
        Clear();

        //AddAllFromDatabase();

        /*
        AddItem(ItemFactory.CreateById("item-important-00001-shard_sorrow"), 1);
        AddItem(ItemFactory.CreateById("item-important-00002-shard_fear"), 1);
        AddItem(ItemFactory.CreateById("item-important-00003-shard_hatred"), 1);
        AddItem(ItemFactory.CreateById("item-important-00004-shard_anger"), 1);
        */

        
        AddItem(ItemFactory.CreateById("item-emblem-00000-a-generic_common"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00000-b-generic_rare"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00001-s-faith_selection"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00002-s-crimson_selection"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00001-faith"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00002-crimson_great"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00003-giant_turtle"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00004-secret_girls"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00005-saint_justice"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00006-marceduke"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00007-great_capitain"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00008-flying_cierva"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00009-atena"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00010-port_virgin"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00011-garden_rangers"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00012-almudena_prayers"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00013-evangegoal"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00014-football_fantasista"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00015-fc_wings"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00001-m-crimson_flood"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00002-m-crimson_crash"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00003-m-crimson_out"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00004-m-crimson_fire"), 1);
        AddItem(ItemFactory.CreateById("item-emblem-00001-e-real_spain_2026"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00001-full_faith"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00002-full_crimson"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00003-full_glide"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00004-full_surge"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00005-full_starlight"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00006-full_barrier"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00007-full_pulse"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00008-full_swift"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00009-full_blaze"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00001-mini_offense"), 1);
        AddItem(ItemFactory.CreateById("item-formation-00002-mini_defense"), 1);
        AddItem(ItemFactory.CreateById("item-kit-00001-faith"), 1);
        AddItem(ItemFactory.CreateById("item-kit-00002-crimson"), 1);
        AddItem(ItemFactory.CreateById("item-kit-00003-balance"), 1);
        AddItem(ItemFactory.CreateById("item-kit-00004-enigma"), 1);
        AddItem(ItemFactory.CreateById("item-kit-00005-hope"), 1);
        
    }

    public void AddAllFromDatabase()
    {
        foreach (ItemData itemData in DatabaseManager.Instance.DatabaseRegistry.ItemData.Data.Values)
        {
            AddItem(ItemFactory.Create(itemData), itemData.MaxStack);
        }
    }

    #endregion

    #region Add / Remove
    public void AddItem(Item item, int count = 1)
    {
        if(TryAddToSystem(item)) return;

        SetSlot(item);

        if (cachedSlot != null)
            cachedSlot.AddCount(count);
        else
            InsertSorted(new ItemStorageSlot(item, count), item.Category);

        ItemEvents.RaiseStorageUpdated();
        LogManager.Trace($"[ItemManagerStorage] Added item {item.ItemId} {count}");
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

    public void Clear()
    {
        foreach (List<ItemStorageSlot> list in categoryMap.Values)
        {
            list.Clear();
        }
    }

    private bool TryAddToSystem(Item item)
    {
        switch (item)
        {
            case ItemCharacter itemCharacter:
                CharacterManager.Instance.AddCharacterFromScout(
                    itemCharacter.CharacterId,
                    1);
                return true;

            case ItemWing itemWing:
                WingManager.Instance.AddWing(
                    DatabaseManager.Instance.GetWingData(itemWing.WingId));
                return true;

            default:
                return false;
        }
    }

    private void InsertSorted(ItemStorageSlot slot, ItemCategory category)
    {
        List<ItemStorageSlot> list = categoryMap[category];

        int low = 0;
        int high = list.Count;

        while (low < high)
        {
            int mid = (low + high) >> 1;

            if (string.Compare(
                    list[mid].Item.ItemId,
                    slot.Item.ItemId,
                    StringComparison.Ordinal) < 0)
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }

        list.Insert(low, slot);
    }

    public bool Buy(Item item, int amount, CurrencyType currencyType)
    {
        if (item == null || amount <= 0) return false;

        int totalCost = item.GetPriceBuy(currencyType) * amount;

        // Spend returns false if there isn't enough currency.
        if (!ItemManager.Instance.Spend(currencyType, totalCost)) return false;

        for (int i = 0; i < amount; i++)
            AddItem(item);

        return true;
    }

    public bool Sell(Item item, int amount, CurrencyType currencyType)
    {
        if (item == null || amount <= 0) return false;

        // Make sure we own enough of the item.
        if (GetItemCount(item) < amount) return false;

        int totalValue = item.GetPriceSell() * amount;

        for (int i = 0; i < amount; i++)
            RemoveItem(item);

        ItemManager.Instance.Add(currencyType, totalValue);

        return true;
    }

    public bool UseItemOnCharacter(Item item, Character target)
    {
        bool success = false;

        switch (item)
        {
            case ItemRecovery itemRecovery:
                target.ModifyBattleStat(Stat.Hp, itemRecovery.RecoveryAmountHp);
                target.ModifyBattleStat(Stat.Sp, itemRecovery.RecoveryAmountSp);
                success = true;
                break;

            case ItemMove itemMove:
                if (!target.CanLearnMove(itemMove.MoveId)) return false;
                target.LearnMove(itemMove.MoveId);
                success = true;
                break;
        }

        if (success)
            RemoveItem(item);

        return success;
    }

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

    public List<ItemStorageSlot> GetItemsByCategory(ItemCategory category) => categoryMap[category];

    public bool IsFormationOfBattleType(Item item, BattleType battleType) 
    {
        if (item.Category != ItemCategory.Formation) return false;

        cachedItemFormation = item as ItemFormation;
        cachedFormation = DatabaseManager.Instance.GetFormation(cachedItemFormation.FormationId);

        return cachedFormation.BattleType == battleType;
    }

    public IReadOnlyDictionary<ItemCategory, List<ItemStorageSlot>> CategoryMap => categoryMap;

    #endregion

    #region Persistence
    
    public SaveDataItemStorage Export()
    {
        SaveDataItemStorage saveData = new SaveDataItemStorage();
        saveData.SaveDataItemStorageSlotList = new List<SaveDataItemStorageSlot>();

        foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
        {
            foreach (ItemStorageSlot slot in categoryMap[category])
            {
                saveData.SaveDataItemStorageSlotList.Add(
                    new SaveDataItemStorageSlot {
                        ItemId = slot.Item.ItemId,
                        Category = slot.Item.Category,
                        Count = slot.Count
                    }         
                );
            }
        }
        return saveData;
    }

    public void Import(SaveDataItemStorage saveData)
    {
        Clear();

        if (saveData?.SaveDataItemStorageSlotList == null) return;

        foreach (SaveDataItemStorageSlot saveDataItemStorageSlot in saveData.SaveDataItemStorageSlotList)
        {
            AddItem(
                ItemFactory.CreateByIdAndCategory(saveDataItemStorageSlot.ItemId, saveDataItemStorageSlot.Category), 
                saveDataItemStorageSlot.Count
            );
        }
    }

    #endregion

}
