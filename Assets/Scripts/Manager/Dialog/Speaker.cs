using UnityEngine;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.SpriteLayer;

public class Speaker
{
    public string SpeakerId { get; private set; }
    public string PortraitCharacterAddress { get; private set; }
    public string PortraitKitAddress { get; private set; }
    public bool HasKit { get; private set; }

    private LocalizationComponentString localizationComponent;
    private LocalizationField localizationField;

    public Speaker (
        string id, 
        LocalizationEntity localizationEntity,
        LocalizationField localizationField,
        string portraitCharacterAddress,
        string portraitKitAddress,
        bool hasKit)
    {
        SpeakerId = id;
        PortraitCharacterAddress = portraitCharacterAddress;
        PortraitKitAddress = portraitKitAddress;
        HasKit = hasKit;

        localizationComponent = new LocalizationComponentString(
            localizationEntity,
            id,
            new[] { localizationField }
        );

        this.localizationField = localizationField;
    }

    // localizationComponent
    public string SpeakerName => localizationComponent.GetString(localizationField);  
}
