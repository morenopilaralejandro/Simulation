using UnityEngine;
using UnityEditor;
using System.IO;
using Simulation.Enums.Kit;

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

            kitData.BaseColorHomeField   = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[baseHomeFieldIndex].Trim()));
            kitData.DetailColorHomeField = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[detailHomeFieldIndex].Trim()));
            kitData.ShockColorHomeField  = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[shockHomeFieldIndex].Trim()));

            kitData.BaseColorHomeKeeper   = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[baseHomeKeeperIndex].Trim()));
            kitData.DetailColorHomeKeeper = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[detailHomeKeeperIndex].Trim()));
            kitData.ShockColorHomeKeeper  = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[shockHomeKeeperIndex].Trim()));

            kitData.BaseColorAwayField   = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[baseAwayFieldIndex].Trim()));
            kitData.DetailColorAwayField = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[detailAwayFieldIndex].Trim()));
            kitData.ShockColorAwayField  = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[shockAwayFieldIndex].Trim()));

            kitData.BaseColorAwayKeeper   = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[baseAwayKeeperIndex].Trim()));
            kitData.DetailColorAwayKeeper = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[detailAwayKeeperIndex].Trim()));
            kitData.ShockColorAwayKeeper  = ColorManager.GetKitColor(EnumManager.StringToEnum<KitColor>(values[shockAwayKeeperIndex].Trim()));

            string safeName = kitData.KitId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(kitData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Kit ScriptableObjects created from CSV.");
    }
}
