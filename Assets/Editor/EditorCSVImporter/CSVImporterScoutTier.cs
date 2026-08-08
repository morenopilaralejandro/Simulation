using UnityEngine;
using UnityEditor;
using System.IO;
using Aremoreno.Enums.Scout;

public class CSVImporterScoutTier
{
    [MenuItem("Tools/Import CSV/Scout/ScoutTier")]
    public static void ImportScoutTierFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressScoutTierData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Scout Tier CSV File", defaultPath, "csv");

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

        string[] headers = lines[0].Split(',');

        int scoutTierIdIndex = System.Array.IndexOf(headers, "ScoutTierId");
        int unlockFlagIndex = System.Array.IndexOf(headers, "UnlockFlag");
        int characterCostIndex = System.Array.IndexOf(headers, "CharacterCost");
        int characterLevelIndex = System.Array.IndexOf(headers, "CharacterLevel");
        int characterIdsIndex = System.Array.IndexOf(headers, "CharacterIds");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            ScoutTierData scoutTierData = ScriptableObject.CreateInstance<ScoutTierData>();

            scoutTierData.ScoutTierId = values[scoutTierIdIndex].Trim();
            scoutTierData.UnlockFlag = values[unlockFlagIndex].Trim();
            scoutTierData.CharacterCost = int.Parse(values[characterCostIndex].Trim());
            scoutTierData.CharacterLevel = int.Parse(values[characterLevelIndex].Trim());
            scoutTierData.CharacterIds = CSVImporterParser.ParseListString(values[characterIdsIndex].Trim());

            string safeName = scoutTierData.ScoutTierId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";

            AssetDatabase.CreateAsset(scoutTierData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("ScoutTier ScriptableObjects created from CSV.");
    }
}
