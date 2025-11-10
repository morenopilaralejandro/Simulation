using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider sliderBgm;
    [SerializeField] private Slider sliderSfx;

    private void Start()
    {
        sliderBgm.value = SettingsManager.Instance.CurrentSettings.BgmVolume;
        sliderSfx.value = SettingsManager.Instance.CurrentSettings.SfxVolume;
    }

    public void OnSliderBgmValueChanged()
    {
        float volume = sliderBgm.value;
        SettingsManager.Instance.SetBgmVolume(volume);
    }

    public void OnSliderSfxValueChanged()
    {
        float volume = sliderSfx.value;
        SettingsManager.Instance.SetSfxVolume(volume);
    }
}
