using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Simulation.Enums.Localization;

public class ComponentLocalizationUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    [SerializeField] private LocalizedString textLocalized;
    [SerializeField] private LocalizedString textRomanized;

    void OnEnable()
    {
        LocalizationManager.Instance?.Subscribe(OnLocalizationStyleChanged);
        ApplyLocalizationStyle(LocalizationManager.Instance.CurrentStyle);
    }

    void OnDisable()
    {
        LocalizationManager.Instance?.Unsubscribe(OnLocalizationStyleChanged);
    }

    private void ApplyLocalizationStyle(LocalizationStyle style)
    {
        if (localizeStringEvent == null)
        {
            LogManager.Warning("[ComponentLocalizationUI] LocalizeStringEvent not assigned in " + gameObject.name);
            return;
        }

        if (style == LocalizationStyle.Localized && textLocalized != null)
            localizeStringEvent.StringReference = textLocalized;
        else if (style == LocalizationStyle.Romanized && textRomanized != null)
            localizeStringEvent.StringReference = textRomanized;

        localizeStringEvent.RefreshString();
    }

    private void OnLocalizationStyleChanged(LocalizationStyle style)
    {
        ApplyLocalizationStyle(style);
    }
}
