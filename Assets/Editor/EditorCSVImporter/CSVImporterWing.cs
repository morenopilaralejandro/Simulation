using UnityEngine;
using UnityEditor;
using System.IO;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class CSVImporterWing
{
    [MenuItem("Tools/Import CSV/Dimension/Wing")]
    public static void ImportWingsFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressDimension/AddressDimensionWing/AddressDimensionWingData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Wing CSV File", defaultPath, "csv");
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

        int wingIdIndex        = System.Array.IndexOf(headers, "WingId");

        int wingTypeIndex       = System.Array.IndexOf(headers, "WingType");
        int wingColorTypeIndex  = System.Array.IndexOf(headers, "WingColorType");

        int elementMainIndex    = System.Array.IndexOf(headers, "ElementMain");
        int elementSubIndex     = System.Array.IndexOf(headers, "ElementSub");

        int wingGrowthTypeIndex = System.Array.IndexOf(headers, "WingGrowthType");
        int wingGrowthRateIndex = System.Array.IndexOf(headers, "WingGrowthRate");

        int kickBaseIndex     = System.Array.IndexOf(headers, "KickBase");
        int bodyBaseIndex     = System.Array.IndexOf(headers, "BodyBase");
        int controlBaseIndex  = System.Array.IndexOf(headers, "ControlBase");
        int guardBaseIndex    = System.Array.IndexOf(headers, "GuardBase");
        int speedBaseIndex    = System.Array.IndexOf(headers, "SpeedBase");
        int staminaBaseIndex  = System.Array.IndexOf(headers, "StaminaBase");
        int techniqueBaseIndex= System.Array.IndexOf(headers, "TechniqueBase");
        int luckBaseIndex     = System.Array.IndexOf(headers, "LuckBase");
        int courageBaseIndex  = System.Array.IndexOf(headers, "CourageBase");

        int kickIndividualIndex     = System.Array.IndexOf(headers, "KickIndividual");
        int bodyIndividualIndex     = System.Array.IndexOf(headers, "BodyIndividual");
        int controlIndividualIndex  = System.Array.IndexOf(headers, "ControlIndividual");
        int guardIndividualIndex    = System.Array.IndexOf(headers, "GuardIndividual");
        int speedIndividualIndex    = System.Array.IndexOf(headers, "SpeedIndividual");
        int staminaIndividualIndex  = System.Array.IndexOf(headers, "StaminaIndividual");
        int techniqueIndividualIndex= System.Array.IndexOf(headers, "TechniqueIndividual");
        int luckIndividualIndex     = System.Array.IndexOf(headers, "LuckIndividual");
        int courageIndividualIndex  = System.Array.IndexOf(headers, "CourageIndividual");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            WingData wingData = ScriptableObject.CreateInstance<WingData>();

            wingData.WingId = values[wingIdIndex].Trim();
			
            wingData.WingType = EnumManager.StringToEnum<WingType>(EnumManager.GetSafeEnumString(values[wingTypeIndex].Trim()));
            wingData.WingColorType  = EnumManager.StringToEnum<WingColorType>(EnumManager.GetSafeEnumString(values[wingColorTypeIndex].Trim()));
            wingData.WingGrowthType = EnumManager.StringToEnum<WingGrowthType>(values[wingGrowthTypeIndex].Trim());
            wingData.WingGrowthRate = EnumManager.StringToEnum<WingGrowthRate>(values[wingGrowthRateIndex].Trim());

            if (string.IsNullOrEmpty(values[elementSubIndex].Trim()))
            {
                wingData.Elements = new Element[1];
                wingData.Elements[0] = EnumManager.StringToEnum<Element>(values[elementMainIndex].Trim());
            }
            else
            {
                wingData.Elements = new Element[2];
                wingData.Elements[0] = EnumManager.StringToEnum<Element>(values[elementMainIndex].Trim());
                wingData.Elements[1] = EnumManager.StringToEnum<Element>(values[elementSubIndex].Trim());
            }

            wingData.KickBase      = int.Parse(values[kickBaseIndex].Trim());
            wingData.BodyBase      = int.Parse(values[bodyBaseIndex].Trim());
            wingData.ControlBase   = int.Parse(values[controlBaseIndex].Trim());
            wingData.GuardBase     = int.Parse(values[guardBaseIndex].Trim());
            wingData.SpeedBase     = int.Parse(values[speedBaseIndex].Trim());
            wingData.StaminaBase   = int.Parse(values[staminaBaseIndex].Trim());
            wingData.TechniqueBase = int.Parse(values[techniqueBaseIndex].Trim());
            wingData.LuckBase      = int.Parse(values[luckBaseIndex].Trim());
            wingData.CourageBase   = int.Parse(values[courageBaseIndex].Trim());

            wingData.KickIndividual      = int.Parse(values[kickIndividualIndex].Trim());
            wingData.BodyIndividual      = int.Parse(values[bodyIndividualIndex].Trim());
            wingData.ControlIndividual   = int.Parse(values[controlIndividualIndex].Trim());
            wingData.GuardIndividual     = int.Parse(values[guardIndividualIndex].Trim());
            wingData.SpeedIndividual     = int.Parse(values[speedIndividualIndex].Trim());
            wingData.StaminaIndividual   = int.Parse(values[staminaIndividualIndex].Trim());
            wingData.TechniqueIndividual = int.Parse(values[techniqueIndividualIndex].Trim());
            wingData.LuckIndividual      = int.Parse(values[luckIndividualIndex].Trim());
            wingData.CourageIndividual   = int.Parse(values[courageIndividualIndex].Trim());

            string safeName = wingData.WingId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(wingData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Wing ScriptableObjects created from CSV.");
    }
}
