using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class OverworldZonePopulator : Editor
{
    private const string ZoneFolderPath = "Assets/Addressables/AddressScene/AddressSceneZone/";

    [MenuItem("Tools/World/Populate Overworld Zones")]
    public static void PopulateAllZones()
    {
        // Find all OverworldDefinition assets in the project
        string[] overworldGuids = AssetDatabase.FindAssets("t:OverworldDefinition");

        if (overworldGuids.Length == 0)
        {
            Debug.LogError("No OverworldDefinition ScriptableObject found in the project.");
            return;
        }

        // Find all ZoneDefinition assets in the specified folder
        string[] zoneGuids = AssetDatabase.FindAssets("t:ZoneDefinition", new[] { ZoneFolderPath });

        if (zoneGuids.Length == 0)
        {
            Debug.LogWarning($"No ZoneDefinition assets found in '{ZoneFolderPath}'.");
            return;
        }

        List<ZoneDefinition> zones = new List<ZoneDefinition>();

        foreach (string guid in zoneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ZoneDefinition zone = AssetDatabase.LoadAssetAtPath<ZoneDefinition>(path);

            if (zone != null)
            {
                zones.Add(zone);
            }
        }

        // Populate each OverworldDefinition found (or just the first one if you prefer)
        foreach (string guid in overworldGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            OverworldDefinition overworld = AssetDatabase.LoadAssetAtPath<OverworldDefinition>(path);

            if (overworld != null)
            {
                Undo.RecordObject(overworld, "Populate Overworld Zones");
                overworld.allZones = zones.ToList();
                EditorUtility.SetDirty(overworld);
                Debug.Log($"Populated '{overworld.name}' with {zones.Count} ZoneDefinition(s).");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Done! Assigned {zones.Count} ZoneDefinition(s) to {overworldGuids.Length} OverworldDefinition(s).");
    }
}
