using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;
using Simulation.Enums.Localization;

public class ComponentLocalization
{
    private Dictionary<LocalizationField, LocalizedString> localizedStrings 
        = new Dictionary<LocalizationField, LocalizedString>();

    public string GetString(LocalizationField field)
    {
        if (localizedStrings.TryGetValue(field, out var localizedString))
            return localizedString.GetLocalizedString();

        LogManager.Warning($"[ComponentLocalization] Missing localized string for field {field}");
        return string.Empty;
    }

    public void Initialize(LocalizationEntity entity, string id, LocalizationField[] fields)
    {
        localizedStrings.Clear();

        foreach (var field in fields)
        {
            localizedStrings[field] = new LocalizedString(
                LocalizationManager.Instance.GetTableReference(entity, field),
                id
            );
        }
    }
}
