using UnityEngine;
using System.IO;

public class PersistanceManager : MonoBehaviour
{
    public static PersistanceManager Instance { get; private set; }

    public const int CURRENT_SAVE_VERSION = 21;

    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
    private static string BackupPath => Path.Combine(Application.persistentDataPath, "save_backup.json");
    private static string TempPath => Path.Combine(Application.persistentDataPath, "save_temp.json");

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

    // =========================
    // SAVE
    // =========================
    public void SaveGame()
    {
        // Create save data (PLACEHOLDER)
        SaveData data = CreateSaveData();
        Save(data);
    }

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

    // =========================
    // LOAD
    // =========================
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

    public void LoadGame() 
    {
        /*
            GetLastSaveData();
            initialize the characters etc.
        */
    }

    // =========================
    // INTERNAL HELPERS
    // =========================
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
            return data != null && data.SaveVersion > 0;
        }
        catch
        {
            return false;
        }
    }

    private SaveData CreateSaveData()
    {
        return new SaveData();
        // TODO: Replace with real game state collection
        /*
        return new SaveData
        {
            saveVersion = CURRENT_SAVE_VERSION,
            debugNote = "Placeholder save data"
        };
        */
    }
}
