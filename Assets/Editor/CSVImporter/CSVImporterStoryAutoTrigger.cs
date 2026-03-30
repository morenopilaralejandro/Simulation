using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVImporterStoryAutoTrigger
{
    [MenuItem("Tools/Import CSV/Story/StoryAutoTrigger")]
    public static void ImportStoryAutoTriggerFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressStoryAutoTriggerData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select StoryAutoTrigger CSV File", defaultPath, "csv");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No CSV file selected.");
            return;
        }

        AssetDatabaseManager.CreateFolderFromPath(assetFolder);

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV file does not contain enough lines.");
            return;
        }

        // Get CSV header index mapping
        string[] headers = lines[0].Split(',');

        int storyAutoTriggerIdIndex = System.Array.IndexOf(headers, "StoryAutoTriggerId");
        int storyEventIdIndex = System.Array.IndexOf(headers, "StoryEventId");
        int storyPrerequisitesIndex = System.Array.IndexOf(headers, "StoryPrerequisites");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            StoryAutoTriggerData storyAutoTriggerData = ScriptableObject.CreateInstance<StoryAutoTriggerData>();

            storyAutoTriggerData.StoryAutoTriggerId = values[storyAutoTriggerIdIndex].Trim();
            storyAutoTriggerData.StoryEventId = values[storyEventIdIndex].Trim();
            storyAutoTriggerData.StoryPrerequisites = CSVImporterParser.ParseListStoryPrerequisite(values[storyPrerequisitesIndex].Trim());

            string safeName = storyAutoTriggerData.StoryAutoTriggerId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(storyAutoTriggerData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("StoryAutoTrigger ScriptableObjects created from CSV.");
    }
}
