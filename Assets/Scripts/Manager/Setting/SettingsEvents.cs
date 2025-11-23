using System;
using Simulation.Enums.Input;
using Simulation.Enums.Localization;

public static class SettingsEvents
{
    public static event Action<int> OnLanguageChanged;
    public static void RaiseLanguageChanged(int localeIndex)
    {
        OnLanguageChanged?.Invoke(localeIndex);
    }

    public static event Action<LocalizationStyle> OnLocalizationStyleChanged;
    public static void RaiseLocalizationStyleChanged(LocalizationStyle style)
    {
        OnLocalizationStyleChanged?.Invoke(style);
    }

    public static event Action<float> OnBgmVolumeChanged;
    public static void RaiseBgmVolumeChanged(float volume)
    {
        OnBgmVolumeChanged?.Invoke(volume);
    }

    public static event Action<float> OnSfxVolumeChanged;
    public static void RaiseSfxVolumeChanged(float volume)
    {
        OnSfxVolumeChanged?.Invoke(volume);
    }

    public static event Action<ControlScheme, ControlSettings> OnControlSchemeChanged;
    public static void RaiseControlSchemeChanged(ControlScheme scheme, ControlSettings controlSettings)
    {
        OnControlSchemeChanged?.Invoke(scheme, controlSettings);
    }

    public static event Action OnSettingsReset;
    public static void RaiseSettingsReset()
    {
        OnSettingsReset?.Invoke();
    }

    public static event Action<bool> OnAutoBattleToggled;
    public static void RaiseAutoBattleToggled(bool enable)
    {
        OnAutoBattleToggled?.Invoke(enable);
    }
}
