using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;

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
