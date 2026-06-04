using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CSVImporterEncounter
{
    [MenuItem("Tools/Import CSV/Encounters")]
    public static void ImportEncountersFromCSV()
    {
        string zoneAssetFolder = "Assets/Addressables/AddressScene/AddressSceneZone";
        string csvFolder = "Csv";
        string defaultPath = Path.Combine(Application.dataPath, csvFolder);
        string path = EditorUtility.OpenFilePanel("Select Encounter CSV File", defaultPath, "csv");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[CSVImporterEncounter] No CSV file selected.");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogWarning("[CSVImporterEncounter] CSV file does not contain enough lines.");
            return;
        }

        // Get CSV header index mapping
        string[] headers = lines[0].Split(',');

        int zoneIdIndex         = System.Array.IndexOf(headers, "ZoneId");
        int teamIdIndex         = System.Array.IndexOf(headers, "TeamId");
        int levelIndex          = System.Array.IndexOf(headers, "Level");
        int encounterRateIndex  = System.Array.IndexOf(headers, "EncounterRate");

        if (zoneIdIndex == -1 || teamIdIndex == -1 || levelIndex == -1 || encounterRateIndex == -1)
        {
            Debug.LogError("[CSVImporterEncounter] CSV is missing one or more required headers: ZoneId, TeamId, Level, EncounterRate");
            return;
        }

        // Group encounters by zoneId
        Dictionary<string, List<EncounterData>> encountersByZone = new Dictionary<string, List<EncounterData>>();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            string zoneId = values[zoneIdIndex].Trim();

            EncounterData encounterData = new EncounterData
            {
                teamId          = values[teamIdIndex].Trim(),
                level           = int.Parse(values[levelIndex].Trim()),
                encounterRate   = float.Parse(values[encounterRateIndex].Trim())
            };

            if (!encountersByZone.ContainsKey(zoneId))
            {
                encountersByZone[zoneId] = new List<EncounterData>();
            }

            encountersByZone[zoneId].Add(encounterData);
        }

        // Find all ZoneDefinition assets in the folder
        string[] guids = AssetDatabase.FindAssets("t:ZoneDefinition", new[] { zoneAssetFolder });

        if (guids.Length == 0)
        {
            Debug.LogWarning($"[CSVImporterEncounter] No ZoneDefinition assets found in {zoneAssetFolder}");
            return;
        }

        int updatedCount = 0;
        HashSet<string> matchedZoneIds = new HashSet<string>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ZoneDefinition zoneDefinition = AssetDatabase.LoadAssetAtPath<ZoneDefinition>(assetPath);

            if (zoneDefinition == null) continue;

            if (encountersByZone.TryGetValue(zoneDefinition.zoneId, out List<EncounterData> encounters))
            {
                zoneDefinition.encounters = new List<EncounterData>(encounters);
                EditorUtility.SetDirty(zoneDefinition);
                matchedZoneIds.Add(zoneDefinition.zoneId);
                updatedCount++;
                Debug.Log($"[CSVImporterEncounter] Updated ZoneDefinition '{zoneDefinition.zoneId}' with {encounters.Count} encounter(s).");
            }
        }

        // Log any zone IDs from CSV that didn't match any asset
        foreach (var zoneId in encountersByZone.Keys)
        {
            if (!matchedZoneIds.Contains(zoneId))
            {
                Debug.LogWarning($"[CSVImporterEncounter] No ZoneDefinition asset found for ZoneId: '{zoneId}'");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CSVImporterEncounter] Encounter import complete. Updated {updatedCount} ZoneDefinition(s).");
    }
}
