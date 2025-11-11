using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System;
using Simulation.Enums.Localization;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private TableConfig tableConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        tableConfig.Initialize();
    }

    private void Start() 
    {
        HandleOnLanguageChanged(SettingsManager.Instance.CurrentSettings.LocaleIndex);
    }

    private void OnEnable()
    {
        SettingsEvents.OnLanguageChanged += HandleOnLanguageChanged;
    }

    private void OnDisable()
    {
        SettingsEvents.OnLanguageChanged += HandleOnLanguageChanged;
    }

    public TableReference GetTableReference(LocalizationEntity entity, LocalizationField field)
    {
        return tableConfig.GetTableReference(
            entity, 
            field, 
            SettingsManager.Instance.CurrentSettings.CurrentLocalizationStyle);
    }

    private void HandleOnLanguageChanged(int localeIndex) 
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
    }
}
