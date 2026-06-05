using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVImporterStoryChapter
{
    [MenuItem("Tools/Import CSV/Story/StoryChapter")]
    public static void ImportStoryChapterFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressStoryChapterData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select StoryChapter CSV File", defaultPath, "csv");
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

        int storyChapterIdIndex = System.Array.IndexOf(headers, "StoryChapterId");
        int storyChapterNumberIndex = System.Array.IndexOf(headers, "StoryChapterNumber");
        int introEventIdIndex = System.Array.IndexOf(headers, "IntroEventId");
        int chapterQuestIdsIndex = System.Array.IndexOf(headers, "ChapterQuestIds");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            StoryChapterData storyChapterData = ScriptableObject.CreateInstance<StoryChapterData>();

            storyChapterData.StoryChapterId = values[storyChapterIdIndex].Trim();
            storyChapterData.StoryChapterNumber = CSVImporterParser.ParseInt(values[storyChapterNumberIndex].Trim());
            storyChapterData.IntroEventId = values[introEventIdIndex].Trim();
            storyChapterData.ChapterQuestIds = CSVImporterParser.ParseListString(values[chapterQuestIdsIndex].Trim());

            string safeName = storyChapterData.StoryChapterId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(storyChapterData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("StoryChapter ScriptableObjects created from CSV.");
    }
}
