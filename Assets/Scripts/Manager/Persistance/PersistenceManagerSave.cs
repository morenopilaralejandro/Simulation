using UnityEngine;
using System;
using System.IO;

public class PersistenceManagerSave : MonoBehaviour
{
    #region Fields

    private PersistenceManager persistenceManager;

    #endregion

    #region Constructor

    public PersistenceManagerSave() 
    {
        persistenceManager = PersistenceManager.Instance;
    }

    #endregion

    #region Logic

    public void SaveGame()
    {
        // Create save data (PLACEHOLDER)
        SaveData data = CreateSaveData();
        persistenceManager.Save(data);
    }

    private SaveData CreateSaveData()
    {
        return null;
        /*
        return new SaveData
        {
            SaveVersion = CURRENT_SAVE_VERSION,
            SaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        */
    }

    #endregion

    #region Helpers

    #endregion

    #region Events
    /*    
    public void Subscribe() { }
    public void Unsubscribe() { }
    */
    #endregion

}
