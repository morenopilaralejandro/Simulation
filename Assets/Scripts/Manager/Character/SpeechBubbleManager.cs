using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Localization;
using System.Threading.Tasks;

public class SpeechBubbleManager : MonoBehaviour
{
    public static SpeechBubbleManager Instance { get; private set; }

    public const float DEFAULT_DURATION = 2.5f;

    private Dictionary<SpeechMessage, Sprite> dictSpeech;
    private LocalizationComponentAsset<Sprite> localizationComponentDirect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateLocalization();
    }

    private void OnEnable()
    {
        SettingsEvents.OnLanguageChanged += HandleLanguageChanged;
        SettingsEvents.OnLocalizationStyleChanged += HandleLocalizationStyleChanged;
    }

    private void OnDisable()
    {
        SettingsEvents.OnLanguageChanged -= HandleLanguageChanged;
        SettingsEvents.OnLocalizationStyleChanged -= HandleLocalizationStyleChanged;
    }

    public Sprite GetSprite(SpeechMessage speechMessage)
    {
        if (dictSpeech.TryGetValue(speechMessage, out Sprite sprite))
            return sprite;
        return null;
    }

    private void UpdateLocalization() 
    {
        localizationComponentDirect = new LocalizationComponentAsset<Sprite>(
            LocalizationEntity.Character,
            EnumManager.EnumToString<SpeechMessage>(SpeechMessage.Direct).ToLower(),
            new[] { LocalizationField.Speech }
        );
        _ = PopulateDict();
    }

    private async Task PopulateDict()
    {
        dictSpeech = new Dictionary<SpeechMessage, Sprite>
        {
            { SpeechMessage.Direct, await localizationComponentDirect.GetAssetAsync(LocalizationField.Speech) }
        };
    }

    public void HandleLanguageChanged(int localeIndex)
    {
        UpdateLocalization();
    }

    public void HandleLocalizationStyleChanged(LocalizationStyle style)
    {
        UpdateLocalization();
    }

}
