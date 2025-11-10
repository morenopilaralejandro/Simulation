using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Simulation.Enums.Localization;

public class LocalizationComponentUISprite : MonoBehaviour
{
    [SerializeField] private LocalizeSpriteEvent localizeSpriteEvent;
    [SerializeField] private LocalizedSprite spriteLocalized;
    [SerializeField] private LocalizedSprite spriteRomanized;

    private void OnEnable()
    {
        SettingsManager.Instance.OnLocalizationStyleChanged += HandleLocalizationStyleChanged;
        ApplyLocalizationStyle(SettingsManager.Instance.CurrentSettings.CurrentLocalizationStyle);
    }

    private void OnDisable()
    {
        SettingsManager.Instance.OnLocalizationStyleChanged -= HandleLocalizationStyleChanged;
    }

    private void ApplyLocalizationStyle(LocalizationStyle style)
    {
        if (localizeSpriteEvent == null)
        {
            Debug.LogWarning("[ComponentLocalizationSprite] LocalizeSpriteEvent not assigned in " + gameObject.name);
            return;
        }

        if (style == LocalizationStyle.Localized && spriteLocalized != null)
        {
            localizeSpriteEvent.AssetReference = spriteLocalized;
        }
        else if (style == LocalizationStyle.Romanized && spriteRomanized != null)
        {
            localizeSpriteEvent.AssetReference = spriteRomanized;
        }
    }

    private void HandleLocalizationStyleChanged(LocalizationStyle style)
    {
        ApplyLocalizationStyle(style);
    }
}
