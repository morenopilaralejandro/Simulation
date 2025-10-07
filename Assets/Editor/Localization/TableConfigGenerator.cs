using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Simulation.Enums.Localization;

public static class TableConfigGenerator
{
    [MenuItem("Tools/Localization/Generate TableConfig")]
    public static void GenerateTableConfig()
    {
        // Find the TableConfig asset
        string[] guids = AssetDatabase.FindAssets("t:TableConfig");
        if (guids.Length == 0)
        {
            Debug.LogError("[TableConfigGenerator] No TableConfig asset found in project!");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        TableConfig config = AssetDatabase.LoadAssetAtPath<TableConfig>(path);

        if (config == null)
        {
            Debug.LogError("[TableConfigGenerator] Failed to load TableConfig asset.");
            return;
        }

        Undo.RecordObject(config, "Generate TableConfig");

        // Clear old mapping
        var mappingsField = typeof(TableConfig).GetField("mappings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var mappings = new List<TableMapping>();

        // Search for all StringTables and AssetTables
        var tables = AssetDatabase.FindAssets("t:StringTableCollection")
            .Concat(AssetDatabase.FindAssets("t:AssetTableCollection"));

        foreach (string guid in tables)
        {
            string tablePath = AssetDatabase.GUIDToAssetPath(guid);

            // Try load as a StringTableCollection
            var stringCollection = AssetDatabase.LoadAssetAtPath<StringTableCollection>(tablePath);
            if (stringCollection != null)
            {
                ProcessCollection(stringCollection.TableCollectionName, mappings);
                continue;
            }

            // Try load as an AssetTableCollection
            var assetCollection = AssetDatabase.LoadAssetAtPath<AssetTableCollection>(tablePath);
            if (assetCollection != null)
            {
                ProcessCollection(assetCollection.TableCollectionName, mappings);
            }
        }

        // Assign new list
        mappingsField.SetValue(config, mappings);

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        Debug.Log($"[TableConfigGenerator] Generated {mappings.Count} mappings into {config.name}");
    }

    private static void ProcessCollection(string tableName, List<TableMapping> mappings)
    {
        if (TryParseTableName(tableName, out LocalizationEntity entity, out LocalizationField field, out LocalizationStyle style))
        {
            TableReference tableRef = tableName;
            mappings.Add(new TableMapping
            {
                Entity = entity,
                Field = field,
                Style = style,
                Table = tableRef
            });
        }
        else
        {
            Debug.LogWarning($"[TableConfigGenerator] Skipping table {tableName}, does not match naming rules.");
        }
    }


    /// <summary>
    /// Parses a table name into (Entity, Field, Style).
    /// Example: Gameplay.Characters.Names.Localized
    /// Naming Rules:
    ///  - Split by capital letter
    ///  - If length &lt; 4 → ignore
    ///  - If length == 4 → use index 1 2 3
    ///  - If length &gt; 6 → ignore
    /// </summary>
    private static bool TryParseTableName(string tableName, out LocalizationEntity entity, out LocalizationField field, out LocalizationStyle style)
    {
        entity = default;
        field = default;
        style = default;

        // Split string by capital letters
        List<string> parts = SplitByDash(tableName);

        if (parts.Count < 4 || parts.Count > 6) {
            return false;
        }
        if (parts.Count == 4)
        {
            return Map(parts[1], parts[2], parts[3], out entity, out field, out style);
        }
        return false;
    }

    private static List<string> SplitByDash(string input)
    {
        return input.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static bool Map(string ent, string fld, string stl,
        out LocalizationEntity entity, out LocalizationField field, out LocalizationStyle style)
    {
        entity = default;
        field = default;
        style = default;

        ent = MakeSingular(ent);
        fld = MakeSingular(fld);

        bool ok = Enum.TryParse(ent, true, out entity) &&
                  Enum.TryParse(fld, true, out field) &&
                  Enum.TryParse(stl, true, out style);

        return ok;
    }

    public static string MakeSingular(string input)
    {
        if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return input.Substring(0, input.Length - 1);
        return input;
    }
}
