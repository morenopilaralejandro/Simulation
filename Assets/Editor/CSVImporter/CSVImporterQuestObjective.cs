using UnityEngine;
using UnityEditor;
using System.IO;
using Aremoreno.Enums.Quest;

public class CSVImporterQuestObjective
{
    [MenuItem("Tools/Import CSV/Quest/QuestObjective")]
    public static void ImportQuestObjectiveFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressQuestObjectiveData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select QuestObjective CSV File", defaultPath, "csv");
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

        int questObjectiveIdIndex = System.Array.IndexOf(headers, "QuestObjectiveId");
        int objectiveTypeIndex = System.Array.IndexOf(headers, "ObjectiveType");
        int targetIdIndex = System.Array.IndexOf(headers, "TargetId");
        int requiredAmountIndex = System.Array.IndexOf(headers, "RequiredAmount");
        int isOptionalIndex = System.Array.IndexOf(headers, "IsOptional");
        int isHiddenIndex = System.Array.IndexOf(headers, "IsHidden");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            QuestObjectiveData questObjectiveData = ScriptableObject.CreateInstance<QuestObjectiveData>();

            questObjectiveData.QuestObjectiveId = values[questObjectiveIdIndex].Trim();
            questObjectiveData.ObjectiveType = EnumManager.StringToEnum<ObjectiveType>(values[objectiveTypeIndex].Trim());
            questObjectiveData.TargetId = values[targetIdIndex].Trim();
            questObjectiveData.RequiredAmount = int.Parse(values[requiredAmountIndex].Trim());
            questObjectiveData.IsOptional = CSVImporterParser.ParseBool(values[isOptionalIndex].Trim());
            questObjectiveData.IsHidden = CSVImporterParser.ParseBool(values[isHiddenIndex].Trim());

            string safeName = questObjectiveData.QuestObjectiveId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(questObjectiveData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("QuestObjective ScriptableObjects created from CSV.");
    }
}
