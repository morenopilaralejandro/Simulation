using System;
using System.IO;
using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.Localization;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public DefaultSettings DefaultSettingsAsset;
    public GameSettings CurrentSettings { get; private set; }

    private string filePath;

    // Events
    public event Action<int> OnLanguageChanged;
    public event Action<LocalizationStyle> OnLocalizationStyleChanged;
    public event Action<float> OnBgmVolumeChanged;
    public event Action<float> OnSfxVolumeChanged;
    public event Action<ControlScheme, ControlSettings> OnControlSchemeChanged;
    public event Action OnSettingsReset;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "settings.json");
        LoadSettings();
    }

    public void LoadSettings()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            CurrentSettings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            CurrentSettings = new GameSettings();
            if (DefaultSettingsAsset != null)
                CurrentSettings = DefaultSettingsAsset.PresetDefault;
        }
    }

    public void SaveSettings()
    {
        File.WriteAllText(filePath, JsonUtility.ToJson(CurrentSettings, true));
    }

    // Global
    public void SetBgmVolume(float volume)
    {
        CurrentSettings.BgmVolume = volume;
        SaveSettings();
        OnBgmVolumeChanged?.Invoke(volume);
    }

    public void SetSfxVolume(float volume)
    {
        CurrentSettings.SfxVolume = volume;
        SaveSettings();
        OnSfxVolumeChanged?.Invoke(volume);
    }

    public void SetLanguage(int localeIndex)
    {
        CurrentSettings.LocaleIndex = localeIndex;
        SaveSettings();
        OnLanguageChanged?.Invoke(localeIndex);
    }

    public void SetLocalizationStyle(LocalizationStyle localizationStyle)
    {
        CurrentSettings.CurrentLocalizationStyle = localizationStyle;
        SaveSettings();
        OnLocalizationStyleChanged?.Invoke(localizationStyle);
    }

    // ======================================
    // Control settings management
    // ======================================

    public void SwitchControlScheme(ControlScheme scheme)
    {
        CurrentSettings.CurrentScheme = scheme;
        SaveSettings();

        var config = GetActiveControlSettings();
        OnControlSchemeChanged?.Invoke(scheme, config);
    }

    public ControlSettings GetActiveControlSettings()
    {
        return CurrentSettings.CurrentScheme switch
        {
            ControlScheme.Touch => CurrentSettings.ControlsSettingsTouch,
            _ => CurrentSettings.ControlsSettingsTraditional
        };
    }

    public void SetAutoPass(bool val)
    {
        var cs = GetActiveControlSettings();
        cs.AutoPass = val;
        SaveSettings();
    }

    public void ResetToDefaults()
    {
        if (DefaultSettingsAsset)
            CurrentSettings = DefaultSettingsAsset.PresetDefault;
        SaveSettings();
        OnSettingsReset?.Invoke();
    }

    public void ApplyPreset(ControlScheme scheme)
    {
        if (DefaultSettingsAsset == null) return;

        if (scheme == ControlScheme.Touch)
            CurrentSettings = DefaultSettingsAsset.PresetTouch;
        else
            CurrentSettings = DefaultSettingsAsset.PresetTraditional;
        
        SaveSettings();
        OnControlSchemeChanged?.Invoke(scheme, GetActiveControlSettings());
    }
}
