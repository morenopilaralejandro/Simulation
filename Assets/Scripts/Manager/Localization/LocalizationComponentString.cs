using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;
using Simulation.Enums.Localization;

public class LocalizationComponentString
{
    /// <summary>
    /// Manages and retrieves localized strings by mapping LocalizationField keys to LocalizedString entries for a specific entity and identifier.
    /// </summary>

    private Dictionary<LocalizationField, LocalizedString> localizedStrings 
        = new Dictionary<LocalizationField, LocalizedString>();


    public LocalizationComponentString(LocalizationEntity entity, string id, LocalizationField[] fields) 
    {
        Initialize(entity, id, fields);
    }

    /// <summary>
    /// Initializes the dictionary of localized strings for a given entity and identifier.
    /// </summary>
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

    /// <summary>
    /// Retrieves the localized string value for the given LocalizationField
    /// </summary>
    public string GetString(LocalizationField field)
    {
        if (localizedStrings.TryGetValue(field, out var localizedString))
            return localizedString.GetLocalizedString();

        Debug.LogWarning($"[LocalizationComponentString] Missing localized string for field {field}");
        return string.Empty;
    }

}

/*
    Example Usage of LocalizationComponentString with Character Data

    // Import required enum namespace
    using MyProject.Enums.Localization;

    // Declare as a serialized field for inspector assignment or reference
    [SerializeField] 
    private LocalizationComponentString localizationStringComponent;

    // Initialization: create and configure the localization component 
    // with the desired entity, identifier, and fields to localize.
    localizationStringComponent = new LocalizationComponentString(
        LocalizationEntity.Character,                     // The type of entity to localize
        characterData.CharacterId,                        // The entry ID in the localization table
        new [] { 
            LocalizationField.Name,                   // Character's full display name
            LocalizationField.Nick,                   // Character's nickname
            LocalizationField.Description                 // Character's description or lore
        }
    );

    // Access localized content
    public string CharacterName         => localizationStringComponent.GetString(LocalizationField.Name);
    public string CharacterNick         => localizationStringComponent.GetString(LocalizationField.Nick);
    public string CharacterDescription  => localizationStringComponent.GetString(LocalizationField.Description);
*/
