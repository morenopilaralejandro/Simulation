using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVImporterKit
{
    [MenuItem("Tools/Import CSV/Kit")]
    public static void ImportKitsFromCSV()
    {
        string assetFolder = "Assets/Addressables/Kits/Data";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Kit CSV File", defaultPath, "csv");
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

        int kitIdIndex = System.Array.IndexOf(headers, "KitId");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            KitData kitData = ScriptableObject.CreateInstance<KitData>();

            kitData.KitId = values[kitIdIndex].Trim();

            string safeName = kitData.KitId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(kitData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Kit ScriptableObjects created from CSV.");
    }
}
