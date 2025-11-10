using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Simulation.Enums.Localization;

public class LocalizationComponentUIText : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    [SerializeField] private LocalizedString textLocalized;
    [SerializeField] private LocalizedString textRomanized;

    void OnEnable()
    {
        SettingsManager.Instance.OnLocalizationStyleChanged += HandleLocalizationStyleChanged;
        ApplyLocalizationStyle(SettingsManager.Instance.CurrentSettings.CurrentLocalizationStyle);
    }

    void OnDisable()
    {
        SettingsManager.Instance.OnLocalizationStyleChanged -= HandleLocalizationStyleChanged;
    }

    private void ApplyLocalizationStyle(LocalizationStyle style)
    {
        if (localizeStringEvent == null)
        {
            Debug.LogWarning("[ComponentLocalizationUI] LocalizeStringEvent not assigned in " + gameObject.name);
            return;
        }

        if (style == LocalizationStyle.Localized && textLocalized != null)
            localizeStringEvent.StringReference = textLocalized;
        else if (style == LocalizationStyle.Romanized && textRomanized != null)
            localizeStringEvent.StringReference = textRomanized;

        localizeStringEvent.RefreshString();
    }

    private void HandleLocalizationStyleChanged(LocalizationStyle style)
    {
        ApplyLocalizationStyle(style);
    }
}
