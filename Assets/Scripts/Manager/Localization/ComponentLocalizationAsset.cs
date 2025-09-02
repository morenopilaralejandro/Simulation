using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Localization;

public class ComponentLocalizationAsset<T> where T : Object
{
    /// <summary>
    /// Manages and retrieves localized assets of type T
    /// by mapping LocalizationField keys to LocalizedAsset entries.
    /// </summary>
    private Dictionary<LocalizationField, LocalizedAsset<T>> localizedAssets 
        = new Dictionary<LocalizationField, LocalizedAsset<T>>();

    public ComponentLocalizationAsset(LocalizationEntity entity, string id, LocalizationField[] fields)
    {
        Initialize(entity, id, fields);
    }

    /// <summary>
    /// Initializes the dictionary of localized assets for the given entity and identifier.
    /// </summary>
    public void Initialize(LocalizationEntity entity, string id, LocalizationField[] fields)
    {
        localizedAssets.Clear();

        foreach (var field in fields)
        {
            var asset = new LocalizedAsset<T>();
            asset.TableReference = LocalizationManager.Instance.GetTableReference(entity, field);
            asset.TableEntryReference = id;  // id must match the entry key in the Asset Table

            localizedAssets[field] = asset;
        }
    }

    /// <summary>
    /// Asynchronously retrieves the localized asset for the given field.
    /// </summary>
    public async Task<T> GetAssetAsync(LocalizationField field)
    {
        if (localizedAssets.TryGetValue(field, out var localizedAsset))
        {
            var handle = localizedAsset.LoadAssetAsync();
            await handle.Task;
            
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            LogManager.Warning($"[ComponentLocalizationAsset] Failed to load localized asset for field {field}");
            return null;
        }

        LogManager.Warning($"[ComponentLocalizationAsset] Missing localized asset for field {field}");
        return null;
    }
}

/*
Usage:
[SerializeField]
private ComponentLocalizationAsset<Sprite> portraitLocalization;

/// Initialize
portraitLocalization = new ComponentLocalizationAsset<Sprite>(
    LocalizationEntity.Character,
    characterData.CharacterId,
    new[] { LocalizationField.Portrait }
);

/// Usage (async)
public async void ApplyPortrait(UnityEngine.UI.Image image)
{
    Sprite portrait = await portraitLocalization.GetAssetAsync(LocalizationField.Portrait);
    if (portrait != null)
        image.sprite = portrait;
}
*/
