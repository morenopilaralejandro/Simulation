using UnityEngine;
using System;
using System.IO;

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager Instance { get; private set; }

    #region Fields

    private PersistenceManagerBackup backupSystem;
    private PersistenceManagerSave saveSystem;
    private PersistenceManagerLoad loadSystem;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        backupSystem = new PersistenceManagerBackup();
        saveSystem = new PersistenceManagerSave();
        loadSystem = new PersistenceManagerLoad();

        //encounterSystem.Subscribe();
    }

    private void OnDestroy() 
    {
        //encounterSystem.Unsubscribe();
    }

    #endregion

    #region API

    // backupSystem
    public int CurrentSaveVersion => PersistenceManagerBackup.CURRENT_SAVE_VERSION;
    public void Save(SaveData saveData) => backupSystem.Save(saveData);
    public SaveData GetLastSaveData() => backupSystem.GetLastSaveData();

    // saveSystem
    public bool IsNewGame() => saveSystem.IsNewGame();
    public void SetNewGame(bool boolValue) => saveSystem.SetNewGame(boolValue);
    public long TimestampCreation => saveSystem.TimestampCreation;
    public void SetTimestampCreation(long longValue) => saveSystem.SetTimestampCreation(longValue);
    public void SaveGame() => saveSystem.SaveGame();

    // loadSystem
    public void LoadGame() => loadSystem.LoadGame();

    #endregion
}
