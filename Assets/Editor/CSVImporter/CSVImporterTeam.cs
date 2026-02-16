using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CSVImporterTeam
{
    [MenuItem("Tools/Import CSV/Team")]
    public static void ImportTeamsFromCSV()
    {
        string assetFolder = "Assets/Addressables/Teams/Data";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Team CSV File", defaultPath, "csv");
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

        int teamIdIndex = System.Array.IndexOf(headers, "TeamId");
        int kitIdIndex = System.Array.IndexOf(headers, "KitId");
        int lvIndex = System.Array.IndexOf(headers, "Lv");
        
        // Full Battle fields
        int fullBattleFormationIdIndex = System.Array.IndexOf(headers, "FullBattleFormationId");
        int fullBattleCharacterId0Index = System.Array.IndexOf(headers, "FullBattleCharacterId0");
        int fullBattleCharacterId1Index = System.Array.IndexOf(headers, "FullBattleCharacterId1");
        int fullBattleCharacterId2Index = System.Array.IndexOf(headers, "FullBattleCharacterId2");
        int fullBattleCharacterId3Index = System.Array.IndexOf(headers, "FullBattleCharacterId3");
        int fullBattleCharacterId4Index = System.Array.IndexOf(headers, "FullBattleCharacterId4");
        int fullBattleCharacterId5Index = System.Array.IndexOf(headers, "FullBattleCharacterId5");
        int fullBattleCharacterId6Index = System.Array.IndexOf(headers, "FullBattleCharacterId6");
        int fullBattleCharacterId7Index = System.Array.IndexOf(headers, "FullBattleCharacterId7");
        int fullBattleCharacterId8Index = System.Array.IndexOf(headers, "FullBattleCharacterId8");
        int fullBattleCharacterId9Index = System.Array.IndexOf(headers, "FullBattleCharacterId9");
        int fullBattleCharacterId10Index = System.Array.IndexOf(headers, "FullBattleCharacterId10");
        int fullBattleCharacterId11Index = System.Array.IndexOf(headers, "FullBattleCharacterId11");
        int fullBattleCharacterId12Index = System.Array.IndexOf(headers, "FullBattleCharacterId12");
        int fullBattleCharacterId13Index = System.Array.IndexOf(headers, "FullBattleCharacterId13");
        int fullBattleCharacterId14Index = System.Array.IndexOf(headers, "FullBattleCharacterId14");
        int fullBattleCharacterId15Index = System.Array.IndexOf(headers, "FullBattleCharacterId15");
        
        // Mini Battle fields
        int miniBattleFormationIdIndex = System.Array.IndexOf(headers, "MiniBattleFormationId");
        int miniBattleCharacterId0Index = System.Array.IndexOf(headers, "MiniBattleCharacterId0");
        int miniBattleCharacterId1Index = System.Array.IndexOf(headers, "MiniBattleCharacterId1");
        int miniBattleCharacterId2Index = System.Array.IndexOf(headers, "MiniBattleCharacterId2");
        int miniBattleCharacterId3Index = System.Array.IndexOf(headers, "MiniBattleCharacterId3");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            TeamData teamData = ScriptableObject.CreateInstance<TeamData>();

            teamData.TeamId = values[teamIdIndex].Trim();
            teamData.KitId = values[kitIdIndex].Trim();
            teamData.Lv = int.Parse(values[lvIndex].Trim());
            
            // Full Battle data
            teamData.FullBattleFormationId = values[fullBattleFormationIdIndex].Trim();
            teamData.FullBattleCharacterIds = new List<string>
            {
                values[fullBattleCharacterId0Index].Trim(),
                values[fullBattleCharacterId1Index].Trim(),
                values[fullBattleCharacterId2Index].Trim(),
                values[fullBattleCharacterId3Index].Trim(),
                values[fullBattleCharacterId4Index].Trim(),
                values[fullBattleCharacterId5Index].Trim(),
                values[fullBattleCharacterId6Index].Trim(),
                values[fullBattleCharacterId7Index].Trim(),
                values[fullBattleCharacterId8Index].Trim(),
                values[fullBattleCharacterId9Index].Trim(),
                values[fullBattleCharacterId10Index].Trim(),
                values[fullBattleCharacterId11Index].Trim(),
                values[fullBattleCharacterId12Index].Trim(),
                values[fullBattleCharacterId13Index].Trim(),
                values[fullBattleCharacterId14Index].Trim(),
                values[fullBattleCharacterId15Index].Trim()
            };
            
            // Mini Battle data
            teamData.MiniBattleFormationId = values[miniBattleFormationIdIndex].Trim();
            teamData.MiniBattleCharacterIds = new List<string>
            {
                values[miniBattleCharacterId0Index].Trim(),
                values[miniBattleCharacterId1Index].Trim(),
                values[miniBattleCharacterId2Index].Trim(),
                values[miniBattleCharacterId3Index].Trim()
            };

            string safeName = teamData.TeamId.Replace(" ", "_").Replace("/", "_");
            string assetPath = $"{assetFolder}/{safeName}.asset";
            AssetDatabase.CreateAsset(teamData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Team ScriptableObjects created from CSV.");
    }
}
