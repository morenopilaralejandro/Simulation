using UnityEngine;
using Simulation.Enums.Localization;

public class Speaker
{
    public string SpeakerId { get; private set; }
    private LocalizationComponentString localizationComponent;
    private CharacterComponentAppearance appearanceComponent;

    public Speaker (
        string id, 
        LocalizationComponentString localizationComponent, 
        CharacterComponentAppearance appearanceComponent)
    {
        this.SpeakerId = id;
        this.localizationComponent = localizationComponent;
        this.appearanceComponent = appearanceComponent;
    }

    // localizationComponent
    public string SpeakerName => localizationComponent.GetString(LocalizationField.Name);
    // appearanceComponent
    public Sprite PortraitSprite => appearanceComponent.PortraitSprite;
}
