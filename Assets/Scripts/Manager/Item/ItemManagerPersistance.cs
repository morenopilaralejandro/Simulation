using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Item;

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

    public SaveDataItemSystem Export()
    {
        SaveDataItemSystem saveData = new SaveDataItemSystem();

        saveData.SaveDataItemStorage = itemManager.ExportStorageSystem();

        return saveData;
    }

    public void Import(SaveDataItemSystem saveData)
    {
        itemManager.ImportStorageSystem(saveData.SaveDataItemStorage);
    }

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
