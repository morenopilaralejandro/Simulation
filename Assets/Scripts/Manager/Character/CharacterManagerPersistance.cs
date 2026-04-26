using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;

public class CharacterManagerPersistance
{
    #region Fields

    private CharacterManager characterManager;

    #endregion

    #region Constructor

    public CharacterManagerPersistance()
    {
        characterManager = CharacterManager.Instance;
    }

    #endregion

    #region Logic

    public CharacterSystemSaveData Export()
    {
        CharacterSystemSaveData saveData = new CharacterSystemSaveData();

        saveData.CharacterStorageSaveData = characterManager.ExportStorageSystem();

        return saveData;
    }

    public void Import(CharacterSystemSaveData saveData)
    {
        characterManager.ImportStorageSystem(saveData.CharacterStorageSaveData);
    }

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
