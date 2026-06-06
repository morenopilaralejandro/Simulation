using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingManagerPersistance
{
    #region Fields

    private WingManager wingManager;

    #endregion

    #region Constructor

    public WingManagerPersistance()
    {
        wingManager = WingManager.Instance;
    }

    #endregion

    #region Logic

    public WingSystemSaveData Export()
    {
        WingSystemSaveData saveData = new WingSystemSaveData();

        saveData.WingStorageSaveData = wingManager.ExportStorageSystem();

        return saveData;
    }

    public void Import(WingSystemSaveData saveData)
    {
        wingManager.ImportStorageSystem(saveData.WingStorageSaveData);
    }

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
