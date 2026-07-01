using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVImporterMatchChain
{
    [MenuItem("Tools/Import CSV/Match/Match Chain")]
    public static void ImportMatchChainsFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressMatchChainData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);

        string path = EditorUtility.OpenFilePanel("Select Match Chain CSV File", defaultPath, "csv");

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

        int matchChainIdIndex = System.Array.IndexOf(headers, "MatchChainId");

        if (matchChainIdIndex == -1)
        {
            Debug.LogError("CSV is missing required column: MatchChainId");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            MatchChainData matchChainData = ScriptableObject.CreateInstance<MatchChainData>();
            matchChainData.MatchChainId = values[matchChainIdIndex].Trim();

            string safeName = matchChainData.MatchChainId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";

            AssetDatabase.CreateAsset(matchChainData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Match Chain ScriptableObjects created from CSV.");
    }
}
