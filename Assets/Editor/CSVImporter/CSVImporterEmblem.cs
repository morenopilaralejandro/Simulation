using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVImporterEmblem
{
    [MenuItem("Tools/Import CSV/Emblem")]
    public static void ImportEmblemsFromCSV()
    {
        string assetFolder = "Assets/Addressables/Emblems/Data";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Emblem CSV File", defaultPath, "csv");
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

        int emblemIdIndex = System.Array.IndexOf(headers, "EmblemId");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            EmblemData emblemData = ScriptableObject.CreateInstance<EmblemData>();

            emblemData.EmblemId = values[emblemIdIndex].Trim();

            string safeName = emblemData.EmblemId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(emblemData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Emblem ScriptableObjects created from CSV.");
    }
}
