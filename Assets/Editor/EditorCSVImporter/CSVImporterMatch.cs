using UnityEngine;
using UnityEditor;
using System.IO;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.World;

public class CSVImporterMatch
{
    [MenuItem("Tools/Import CSV/Match/Match")]
    public static void ImportMatchFromCSV()
    {
        string assetFolder = "Assets/Addressables/AddressMatchData";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Match CSV File", defaultPath, "csv");

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

        int matchIdIndex = System.Array.IndexOf(headers, "MatchId");
        int teamIdIndex = System.Array.IndexOf(headers, "TeamId");
        int levelIndex = System.Array.IndexOf(headers, "Level");
        int battleTypeIndex = System.Array.IndexOf(headers, "BattleType");
        int bgmIdIndex = System.Array.IndexOf(headers, "BgmId");
        int ballIdIndex = System.Array.IndexOf(headers, "BallId");
        int fieldIdIndex = System.Array.IndexOf(headers, "FieldId");
        int dropIdAIndex = System.Array.IndexOf(headers, "DropIdA");
        int dropIdBIndex = System.Array.IndexOf(headers, "DropIdB");
        int dropIdSIndex = System.Array.IndexOf(headers, "DropIdS");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            MatchData matchData = ScriptableObject.CreateInstance<MatchData>();

            matchData.MatchId = values[matchIdIndex].Trim();
            matchData.TeamId = values[teamIdIndex].Trim();
            matchData.Level = int.Parse(values[levelIndex].Trim());
            matchData.BattleType = EnumManager.StringToEnum<BattleType>(values[battleTypeIndex].Trim());
            matchData.BgmId = values[bgmIdIndex].Trim();
            matchData.BallId = values[ballIdIndex].Trim();
            matchData.FieldId = values[fieldIdIndex].Trim();
            matchData.DropIdA = values[dropIdAIndex].Trim();
            matchData.DropIdB = values[dropIdBIndex].Trim();
            matchData.DropIdS = values[dropIdSIndex].Trim();

            string safeName = matchData.MatchId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(matchData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Match ScriptableObjects created from CSV.");
    }
}
