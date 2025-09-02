using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Simulation.Enums.Localization;

[CreateAssetMenu(fileName = "TableConfig", menuName = "Localization/TableConfig")]
public class TableConfig : ScriptableObject
{
    [SerializeField] private List<TableMapping> mappings = new();

    private Dictionary<(LocalizationEntity, LocalizationField, LocalizationStyle), TableReference> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<(LocalizationEntity, LocalizationField, LocalizationStyle), TableReference>();
        foreach (var mapping in mappings)
        {
            var key = (mapping.Entity, mapping.Field, mapping.Style);
            if (!lookup.ContainsKey(key))
                lookup.Add(key, mapping.Table);
            else
                LogManager.Warning($"[TableConfig] Duplicate mapping found for {key} in {this.name}");
        }
    }

    public TableReference GetTableReference(LocalizationEntity entity, LocalizationField field, LocalizationStyle style)
    {
        if (lookup == null)
            Initialize();

        if (lookup.TryGetValue((entity, field, style), out TableReference table))
            return table;

        LogManager.Warning($"[TableConfig] No StringTable mapping for {entity}-{field}-{style}");
        return default;
    }
}
