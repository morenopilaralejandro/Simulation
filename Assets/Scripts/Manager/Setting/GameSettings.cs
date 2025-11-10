using Simulation.Enums.Input;
using Simulation.Enums.Localization;

[System.Serializable]
public class GameSettings
{
    // Shared/global
    public int LocaleIndex = 0;
    public LocalizationStyle CurrentLocalizationStyle = LocalizationStyle.Localized;
    public float BgmVolume = 1f;
    public float SfxVolume = 1f;
    
    // Active scheme selector
    public ControlScheme CurrentScheme = ControlScheme.Traditional;

    // Scheme-specific settings
    public ControlSettings ControlsSettingsTraditional = new ControlSettings();
    public ControlSettings ControlsSettingsTouch = new ControlSettings();
}
