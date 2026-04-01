using UnityEngine;
using System;
using System.IO;

public class PersistenceManagerBackup : MonoBehaviour
{
    #region Fields

    public const int CURRENT_SAVE_VERSION = 2;

    private string SavePath   => Path.Combine(Application.persistentDataPath, "save.json");
    private string BackupPath => Path.Combine(Application.persistentDataPath, "save_backup.json");
    private string TempPath   => Path.Combine(Application.persistentDataPath, "save_temp.json");

    #endregion

    #region Constructor

    public PersistenceManagerBackup() { }

    #endregion

    #region Save

    public void Save(SaveData saveData) 
    {
        string json = JsonUtility.ToJson(saveData, true);
        try
        {
            // Write temp file
            File.WriteAllText(TempPath, json);

            // Validate temp save
            if (!TryValidateSave(TempPath))
            {
                LogManager.Error("[PersistenceManager] Save validation failed.");
                return;
            }

            // Create backup
            if (File.Exists(SavePath))
                File.Copy(SavePath, BackupPath, overwrite: true);

            // Replace save with temp
            File.Copy(TempPath, SavePath, overwrite: true);
            File.Delete(TempPath);

            LogManager.Trace("[PersistenceManager] Game saved successfully.");
        }
        catch (System.Exception e)
        {
            LogManager.Error($"[PersistenceManager] Save failed: {e}");
        }
    }

    #endregion

    #region Load

    public SaveData GetLastSaveData()
    {
        // Try main save
        if (TryLoad(SavePath, out SaveData data))
        {
            LogManager.Trace("[PersistenceManager] Loaded main save.");
            return data;
        }

        // Fallback to backup
        if (TryLoad(BackupPath, out data))
        {
            LogManager.Warning("[PersistenceManager] Main save failed. Loaded backup.");
            return data;
        }

        LogManager.Warning("[PersistenceManager] No valid save found.");
        return null;
    }

    #endregion

    #region Helpers

    private bool TryLoad(string path, out SaveData data)
    {
        data = null;

        if (!File.Exists(path)) return false;

        try
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(json);
            return data != null;
        }
        catch
        {
            return false;
        }
    }

    private bool TryValidateSave(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data != null && data.VersionNumber > 0;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Events
    /*    
    public void Subscribe() { }
    public void Unsubscribe() { }
    */
    #endregion

}
