using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using System;
using Simulation.Enums.Localization;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private StringTableConfig stringTableConfig;
    [SerializeField] private LocalizationStyle currentStyle = LocalizationStyle.Localized;
    public LocalizationStyle CurrentStyle => currentStyle;

    public event Action<LocalizationStyle> OnLocalizationStyleChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        stringTableConfig.Initialize();
    }

    public void Subscribe(Action<LocalizationStyle> callback)
    {
        OnLocalizationStyleChanged += callback;
    }

    public void Unsubscribe(Action<LocalizationStyle> callback)
    {
        OnLocalizationStyleChanged -= callback;
    }

    public void SetLocalizationStyle(LocalizationStyle style)
    {
        currentStyle = style;
        OnLocalizationStyleChanged?.Invoke(style);
    }

    public TableReference GetTableReference(LocalizationEntity entity, LocalizationField field)
    {
        return stringTableConfig.GetTableReference(entity, field, currentStyle);
    }
}
