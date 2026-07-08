using UnityEngine;
using UnityEditor;
using System.IO;
using Aremoreno.Enums.Item;

public class CSVImporterShop
{
    [MenuItem("Tools/Import CSV/Item/Shop")]
    public static void ImportShopFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressShopData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Shop CSV File", defaultPath, "csv");

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

        int shopIdIndex = System.Array.IndexOf(headers, "ShopId");
        int currencyTypeIndex = System.Array.IndexOf(headers, "CurrencyType");
        int itemIdsIndex = System.Array.IndexOf(headers, "ItemIds");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');

            ShopData shopData = ScriptableObject.CreateInstance<ShopData>();

            shopData.ShopId = values[shopIdIndex].Trim();
            shopData.CurrencyType = EnumManager.StringToEnum<CurrencyType>(values[currencyTypeIndex].Trim());
            shopData.ItemIds = CSVImporterParser.ParseListString(values[itemIdsIndex].Trim());

            string safeName = shopData.ShopId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";

            AssetDatabase.CreateAsset(shopData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Shop ScriptableObjects created from CSV.");
    }
}
