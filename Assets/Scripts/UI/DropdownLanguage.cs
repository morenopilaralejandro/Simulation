using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class DropdownLanguage : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdownLanguage;

    private int initialLanguageIndex;

    void Start()
    {
        InitializeDropdown();
    }

    public void InitializeDropdown()
    {
        dropdownLanguage.ClearOptions();
        List<string> options = new List<string>();
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            options.Add(locale.Identifier.CultureInfo.NativeName);
        }
        dropdownLanguage.AddOptions(options);

        // Use SettingsManager to retrieve the language index
        int defaultLanguageIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        int savedLanguageIndex = SettingsManager.Instance.CurrentSettings.LocaleIndex;

        // Fallback: If saved index is out of bounds (e.g. localization changed), use default
        if (savedLanguageIndex < 0 || savedLanguageIndex >= LocalizationSettings.AvailableLocales.Locales.Count)
            savedLanguageIndex = defaultLanguageIndex;

        dropdownLanguage.value = savedLanguageIndex;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[savedLanguageIndex];
        initialLanguageIndex = savedLanguageIndex;
    }

    public void OnValueChanged() {
        //AudioManager.Instance.PlaySfx("SfxMenuChange");
        int selectedIndex = dropdownLanguage.value;
        SettingsManager.Instance.SetLanguage(selectedIndex);
    }
}
