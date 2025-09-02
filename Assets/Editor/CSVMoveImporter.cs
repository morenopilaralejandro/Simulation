using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class CSVMoveImporter
{
    [MenuItem("Tools/Import CSV/Move")]
    public static void ImportMovesFromCSV()
    {
        string assetFolder = "Assets/ScriptableObjects/Move";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Move CSV File", defaultPath, "csv");
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

        // --- Map headers dynamically ---
        string[] headers = lines[0].Split(',');

        int moveIdIndex      = System.Array.IndexOf(headers, "MoveId");
        int categoryIndex    = System.Array.IndexOf(headers, "Category");
        int elementIndex     = System.Array.IndexOf(headers, "Element");
        int traitIndex       = System.Array.IndexOf(headers, "Trait");
        int growthTypeIndex  = System.Array.IndexOf(headers, "GrowthType");
        int growthRateIndex  = System.Array.IndexOf(headers, "GrowthRate");
        int costIndex        = System.Array.IndexOf(headers, "Cost");
        int basePowerIndex   = System.Array.IndexOf(headers, "BasePower");
        int stunDamageIndex  = System.Array.IndexOf(headers, "StunDamage");
        int auraDamageIndex  = System.Array.IndexOf(headers, "AuraDamage");
        int difficultyIndex  = System.Array.IndexOf(headers, "Difficulty");
        int faultRateIndex   = System.Array.IndexOf(headers, "FaultRate");
        int participantsIndex= System.Array.IndexOf(headers, "Participants");

        int allowedElementsIndex  = System.Array.IndexOf(headers, "AllowedElements");
        int allowedPositionsIndex = System.Array.IndexOf(headers, "AllowedPositions");
        int allowedGendersIndex   = System.Array.IndexOf(headers, "AllowedGenders");
        int allowedSizesIndex     = System.Array.IndexOf(headers, "AllowedSizes");

        int requiredParticipantElementsIndex = System.Array.IndexOf(headers, "RequiredParticipantElements");
        int requiredParticipantMovesIndex    = System.Array.IndexOf(headers, "RequiredParticipantMoves");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            MoveData moveData = ScriptableObject.CreateInstance<MoveData>();

            moveData.MoveId        = values[moveIdIndex].Trim();
            moveData.Category      = EnumManager.StringToEnum<Category>(values[categoryIndex].Trim());
            moveData.Element       = EnumManager.StringToEnum<Element>(values[elementIndex].Trim());
            moveData.Trait         = EnumManager.StringToEnum<Trait>(values[traitIndex].Trim());
            moveData.GrowthType    = EnumManager.StringToEnum<GrowthType>(values[growthTypeIndex].Trim());
            moveData.GrowthRate    = EnumManager.StringToEnum<GrowthRate>(values[growthRateIndex].Trim());

            moveData.Cost          = int.Parse(values[costIndex].Trim());
            moveData.BasePower     = int.Parse(values[basePowerIndex].Trim());
            moveData.StunDamage    = int.Parse(values[stunDamageIndex].Trim());
            moveData.AuraDamage    = int.Parse(values[auraDamageIndex].Trim());
            moveData.Difficulty    = int.Parse(values[difficultyIndex].Trim());
            moveData.FaultRate     = int.Parse(values[faultRateIndex].Trim());
            moveData.Participants  = int.Parse(values[participantsIndex].Trim());

            // --- Lists (split by '|') ---
            moveData.AllowedElements  = EnumManager.ParseEnumList<Element>(values[allowedElementsIndex].Trim());
            moveData.AllowedPositions = EnumManager.ParseEnumList<Position>(values[allowedPositionsIndex].Trim());
            moveData.AllowedGenders   = EnumManager.ParseEnumList<Gender>(values[allowedGendersIndex].Trim());
            moveData.AllowedSizes     = EnumManager.ParseEnumList<CharacterSize>(values[allowedSizesIndex].Trim());

            moveData.RequiredParticipantElements = EnumManager.ParseEnumList<Element>(values[requiredParticipantElementsIndex].Trim());
            moveData.RequiredParticipantMoves    = EnumManager.ParseStringList(values[requiredParticipantMovesIndex].Trim());

            // Save asset
            string safeName = moveData.MoveId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(moveData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Move ScriptableObjects created from CSV.");
    }
}
