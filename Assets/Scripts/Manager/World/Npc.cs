using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.SpriteLayer;

public class Npc
{
    #region Components

    private NpcComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Initialize

    public Npc(NpcData data) 
    {
        Initialize(data);
    }

    public void Initialize(NpcData data)
    {
        attributesComponent = new NpcComponentAttributes(data);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Npc,
            data.NpcId,
            new[] { LocalizationField.Name }
        );
        appearanceComponent = new CharacterComponentAppearance(data);
    }

    #endregion

    #region API
    // attributesComponent
    public string NpcId => attributesComponent.NpcId;
    public Gender Gender => attributesComponent.Gender;
    // localizationComponent
    public LocalizationComponentString LocalizationComponent => localizationStringComponent;
    public string NpcName => localizationStringComponent.GetString(LocalizationField.Name);
    // appearanceComponent
    public CharacterComponentAppearance AppearanceComponent => appearanceComponent;
    public Sprite PortraitSprite => appearanceComponent.PortraitSprite;
    public string PortraitSpriteId => appearanceComponent.PortraitSpriteId;
    public PortraitSize PortraitSize => appearanceComponent.PortraitSize;
    public HairStyle HairStyle => appearanceComponent.HairStyle;
    public HairColorType HairColorType => appearanceComponent.HairColorType;
    public EyeColorType EyeColorType => appearanceComponent.EyeColorType;
    public BodyColorType BodyColorType => appearanceComponent.BodyColorType;
    public SpriteLayerState<CharacterSpriteLayer> SpriteLayerState
    {
        get => appearanceComponent.State;
        set => appearanceComponent.State = value;
    }
    public void ApplyKit(Kit kit, Variant variant, Position position) => appearanceComponent.ApplyKit(kit, variant, position);
    public void InitializeVisibility() => appearanceComponent.InitializeVisibility();
    public Variant GetKitVariant(Team team) => appearanceComponent.GetKitVariant(team);
    public Role GetKitRole(Position position) => appearanceComponent.GetKitRole(position);
    #endregion
}
