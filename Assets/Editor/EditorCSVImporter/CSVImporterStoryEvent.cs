using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVImporterStoryEvent
{
    [MenuItem("Tools/Import CSV/Story/StoryEvent")]
    public static void ImportStoryEventFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressStoryEventData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select StoryEvent CSV File", defaultPath, "csv");
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

        int storyEventIdIndex = System.Array.IndexOf(headers, "StoryEventId");
        int storyPrerequisitesIndex = System.Array.IndexOf(headers, "StoryPrerequisites");
        int storyEffectsIndex = System.Array.IndexOf(headers, "StoryEffects");
        int cutsceneIdIndex = System.Array.IndexOf(headers, "CutsceneId");
        int scriptedEventIdIndex = System.Array.IndexOf(headers, "ScriptedEventId");
        int bgmIdIndex = System.Array.IndexOf(headers, "BgmId");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            StoryEventData storyEventData = ScriptableObject.CreateInstance<StoryEventData>();

            storyEventData.StoryEventId = values[storyEventIdIndex].Trim();
            storyEventData.StoryPrerequisites = CSVImporterParser.ParseListStoryPrerequisite(values[storyPrerequisitesIndex].Trim());
            storyEventData.StoryEffects = CSVImporterParser.ParseListStoryEffect(values[storyEffectsIndex].Trim());
            storyEventData.CutsceneId = values[cutsceneIdIndex].Trim();
            storyEventData.ScriptedEventId = values[scriptedEventIdIndex].Trim();
            storyEventData.BgmId = values[bgmIdIndex].Trim();

            string safeName = storyEventData.StoryEventId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(storyEventData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("StoryEvent ScriptableObjects created from CSV.");
    }
}
