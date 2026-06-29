using UnityEngine;
using UnityEditor;
using System.IO;
using Aremoreno.Enums.Field;

public class CSVImporterField
{
    [MenuItem("Tools/Import CSV/Field/Field")]
    public static void ImportFieldFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressFieldData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Field CSV File", defaultPath, "csv");
        
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

        int fieldIdIndex = System.Array.IndexOf(headers, "FieldId");
        int fieldLineColorIndex = System.Array.IndexOf(headers, "FieldLineColor");
        int textureInnerAddressIndex = System.Array.IndexOf(headers, "TextureInnerAddress");
        int textureOuterAddressIndex = System.Array.IndexOf(headers, "TextureOuterAddress");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            FieldData fieldData = ScriptableObject.CreateInstance<FieldData>();

            fieldData.FieldId = values[fieldIdIndex].Trim();
            fieldData.FieldLineColor = EnumManager.StringToEnum<FieldLineColor>(values[fieldLineColorIndex].Trim());
            fieldData.TextureInnerAddress = values[textureInnerAddressIndex].Trim();
            fieldData.TextureOuterAddress = values[textureOuterAddressIndex].Trim();

            string safeName = fieldData.FieldId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(fieldData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Field ScriptableObjects created from CSV.");
    }
}
