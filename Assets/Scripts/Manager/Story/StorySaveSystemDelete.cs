using UnityEngine;
using System.IO;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public static class StorySaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "story_progress.json");

    public static void Save()
    {
        var data = StoryProgressManager.Instance.GetSaveData();
        string json = JsonUtility.ToJson(data, true);

        // JsonUtility doesn't handle Dictionaries well, so use a JSON lib or convert
        // For production, use Newtonsoft JSON or a custom serializer
        File.WriteAllText(SavePath, json);
        Debug.Log($"[Save] Story progress saved to {SavePath}");
    }

    public static void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("[Save] No save file found.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<StoryProgressSaveData>(json);
        StoryProgressManager.Instance.LoadSaveData(data);
        Debug.Log("[Save] Story progress loaded.");
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
