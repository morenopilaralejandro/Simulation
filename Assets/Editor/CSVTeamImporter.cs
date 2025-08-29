using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CSVTeamImporter
{
    [MenuItem("Tools/Import CSV/Team")]
    public static void ImportTeamsFromCSV()
    {
       
        string assetFolder = "Assets/Resources/ScriptableObjects/Team";
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

        int teamIdIndex         = System.Array.IndexOf(headers, "TeamId");
        int formationIdIndex    = System.Array.IndexOf(headers, "FormationId");
        int kitIdIndex          = System.Array.IndexOf(headers, "KitId");
        int lvIndex             = System.Array.IndexOf(headers, "Lv");
        int characterId0Index   = System.Array.IndexOf(headers, "CharacterId0");
        int characterId1Index   = System.Array.IndexOf(headers, "CharacterId1");
        int characterId2Index   = System.Array.IndexOf(headers, "CharacterId2");
        int characterId3Index   = System.Array.IndexOf(headers, "CharacterId3");
        int characterId4Index   = System.Array.IndexOf(headers, "CharacterId4");
        int characterId5Index   = System.Array.IndexOf(headers, "CharacterId5");
        int characterId6Index   = System.Array.IndexOf(headers, "CharacterId6");
        int characterId7Index   = System.Array.IndexOf(headers, "CharacterId7");
        int characterId8Index   = System.Array.IndexOf(headers, "CharacterId8");
        int characterId9Index   = System.Array.IndexOf(headers, "CharacterId9");
        int characterId10Index  = System.Array.IndexOf(headers, "CharacterId10");
        int characterId11Index  = System.Array.IndexOf(headers, "CharacterId11");
        int characterId12Index  = System.Array.IndexOf(headers, "CharacterId12");
        int characterId13Index  = System.Array.IndexOf(headers, "CharacterId13");
        int characterId14Index  = System.Array.IndexOf(headers, "CharacterId14");
        int characterId15Index  = System.Array.IndexOf(headers, "CharacterId15");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            TeamData teamData = ScriptableObject.CreateInstance<TeamData>();

            teamData.TeamId         = values[teamIdIndex].Trim();
            teamData.FormationId    = values[formationIdIndex].Trim();
            teamData.KitId          = values[kitIdIndex].Trim();
            teamData.Lv             = int.Parse(values[lvIndex].Trim());
            teamData.CharacterIds   = new List<string>
            {
                values[characterId0Index].Trim(),
                values[characterId1Index].Trim(),
                values[characterId2Index].Trim(),
                values[characterId3Index].Trim(),
                values[characterId4Index].Trim(),
                values[characterId5Index].Trim(),
                values[characterId6Index].Trim(),
                values[characterId7Index].Trim(),
                values[characterId8Index].Trim(),
                values[characterId9Index].Trim(),
                values[characterId10Index].Trim(),
                values[characterId11Index].Trim(),
                values[characterId12Index].Trim(),
                values[characterId13Index].Trim(),
                values[characterId14Index].Trim(),
                values[characterId15Index].Trim()
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
