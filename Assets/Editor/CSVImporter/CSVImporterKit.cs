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

        int baseHomeFieldIndex = System.Array.IndexOf(headers, "BaseColorHomeField");
        int detailHomeFieldIndex = System.Array.IndexOf(headers, "DetailColorHomeField");
        int shockHomeFieldIndex = System.Array.IndexOf(headers, "ShockColorHomeField");

        int baseHomeKeeperIndex = System.Array.IndexOf(headers, "BaseColorHomeKeeper");
        int detailHomeKeeperIndex = System.Array.IndexOf(headers, "DetailColorHomeKeeper");
        int shockHomeKeeperIndex = System.Array.IndexOf(headers, "ShockColorHomeKeeper");

        int baseAwayFieldIndex = System.Array.IndexOf(headers, "BaseColorAwayField");
        int detailAwayFieldIndex = System.Array.IndexOf(headers, "DetailColorAwayField");
        int shockAwayFieldIndex = System.Array.IndexOf(headers, "ShockColorAwayField");

        int baseAwayKeeperIndex = System.Array.IndexOf(headers, "BaseColorAwayKeeper");
        int detailAwayKeeperIndex = System.Array.IndexOf(headers, "DetailColorAwayKeeper");
        int shockAwayKeeperIndex = System.Array.IndexOf(headers, "ShockColorAwayKeeper");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            KitData kitData = ScriptableObject.CreateInstance<KitData>();

            kitData.KitId = values[kitIdIndex].Trim();

            kitData.BaseColorHomeField   = ParseColor(values, baseHomeFieldIndex);
            kitData.DetailColorHomeField = ParseColor(values, detailHomeFieldIndex);
            kitData.ShockColorHomeField  = ParseColor(values, shockHomeFieldIndex);

            kitData.BaseColorHomeKeeper   = ParseColor(values, baseHomeKeeperIndex);
            kitData.DetailColorHomeKeeper = ParseColor(values, detailHomeKeeperIndex);
            kitData.ShockColorHomeKeeper  = ParseColor(values, shockHomeKeeperIndex);

            kitData.BaseColorAwayField   = ParseColor(values, baseAwayFieldIndex);
            kitData.DetailColorAwayField = ParseColor(values, detailAwayFieldIndex);
            kitData.ShockColorAwayField  = ParseColor(values, shockAwayFieldIndex);

            kitData.BaseColorAwayKeeper   = ParseColor(values, baseAwayKeeperIndex);
            kitData.DetailColorAwayKeeper = ParseColor(values, detailAwayKeeperIndex);
            kitData.ShockColorAwayKeeper  = ParseColor(values, shockAwayKeeperIndex);


            string safeName = kitData.KitId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(kitData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Kit ScriptableObjects created from CSV.");
    }
}
