using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Aremoreno.Enums.Input;

public class MenuMainPanelSettings : Menu
{
    [Header("UI References")]
    [SerializeField] private DropdownLanguage dropdownLanguage;
    [SerializeField] private Slider sliderBgm;
    [SerializeField] private Slider sliderSfx;

    private float cachedBgmVolume;
    private float cachedSfxVolume;
    private int cachedLanguageIndex;

    public override void Show() 
    {
        base.Show();

        dropdownLanguage.InitializeDropdown();
        CacheSettings();
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    public void OnButtonConfirmClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");    
        RequestClose();
    }

    public void OnButtonCancelClicked()
    {
        RequestClose();
        RestoreSettings();
    }

    /*

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamPanelNameOpened += HandleTeamPanelNameOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamPanelNameOpened -= HandleTeamPanelNameOpened;
    }

    private void HandleTeamPanelNameOpened(string teamName)
    {
        inputFieldName.text = teamName;
        MenuManager.Instance.OpenMenu(this);
    }

    */

    private void CacheSettings() 
    {
        cachedBgmVolume = SettingsManager.Instance.CurrentSettings.BgmVolume;
        cachedSfxVolume = SettingsManager.Instance.CurrentSettings.SfxVolume;

        int defaultLanguageIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        int savedLanguageIndex = SettingsManager.Instance.CurrentSettings.LocaleIndex;

        // Fallback: If saved index is out of bounds (e.g. localization changed), use default
        if (savedLanguageIndex < 0 || savedLanguageIndex >= LocalizationSettings.AvailableLocales.Locales.Count)
            savedLanguageIndex = defaultLanguageIndex;

        cachedLanguageIndex = savedLanguageIndex;
    }

    private void RestoreSettings()
    {
        SettingsManager.Instance.SetBgmVolume(cachedBgmVolume);
        SettingsManager.Instance.SetSfxVolume(cachedSfxVolume);
        SettingsManager.Instance.SetLanguage(cachedLanguageIndex);

        sliderBgm.value = cachedBgmVolume;
        sliderSfx.value = cachedSfxVolume;
    }
}
