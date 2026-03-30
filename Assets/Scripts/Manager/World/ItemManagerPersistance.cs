using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Item;

public class ItemManagerPersistance
{
    #region Fields

    private ItemManager itemManager;

    #endregion

    #region Constructor

    public ItemManagerPersistance()
    {
        itemManager = ItemManager.Instance;
    }

    #endregion

    #region Logic

    public ItemSystemSaveData Export()
    {
        ItemSystemSaveData saveData = new ItemSystemSaveData();

        saveData.ItemStorageSaveData = itemManager.ExportStorageSystem();

        return saveData;
    }

    public void Import(ItemSystemSaveData saveData)
    {
        itemManager.ImportStorageSystem(saveData.ItemStorageSaveData);
    }

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
