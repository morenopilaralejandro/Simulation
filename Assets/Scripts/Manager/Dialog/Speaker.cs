using UnityEngine;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.SpriteLayer;

public class Speaker
{
    public string SpeakerId { get; private set; }
    private LocalizationComponentString localizationComponent;
    private CharacterComponentAppearance appearanceComponent;
    public bool HasKit { get; private set; }
    private KitManager kitManager;

    public Speaker (
        string id, 
        LocalizationComponentString localizationComponent, 
        CharacterComponentAppearance appearanceComponent,
        DialogKit dialogKit)
    {
        this.SpeakerId = id;
        this.localizationComponent = localizationComponent;
        this.appearanceComponent = appearanceComponent;

        kitManager = KitManager.Instance;
        this.HasKit = dialogKit != null && dialogKit.KitId != "none";
        if(this.HasKit)
            ApplyKit(dialogKit);
    }

    // localizationComponent
    public string SpeakerName => localizationComponent.GetString(LocalizationField.Name);
    // appearanceComponent
    public CharacterComponentAppearance AppearanceComponent => appearanceComponent;
    public Sprite PortraitSprite => appearanceComponent.PortraitSprite;
    public PortraitSize PortraitSize => appearanceComponent.PortraitSize;
    public SpriteLayerState<CharacterSpriteLayer> SpriteLayerState
    {
        get => appearanceComponent.State;
        set => appearanceComponent.State = value;
    }
    //public void ApplyKit(Kit kit, Variant variant, Position position) => appearanceComponent.ApplyKit(kit, variant, position);
    //public void ApplyKit(Kit kit, Variant variant, Role role) => appearanceComponent.ApplyKit(kit, variant, role);
    public void ApplyKit(DialogKit dialogKit)
    {
        Kit kit = null; 
        Variant variant = EnumManager.StringToEnum<Variant>(dialogKit.VariantId);
        Role role = EnumManager.StringToEnum<Role>(dialogKit.RoleId);

        if (dialogKit.KitId == "default")
        {
            //kit = get user selected kit
        } else {
            kit = kitManager.GetKit(dialogKit.KitId);
        }

        appearanceComponent.ApplyKit(kit, variant, role);

    }
}
