using UnityEngine;
using System;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.SpriteLayer;

public class CharacterComponentAppearance
{
    #region Fields

    public string PortraitSpriteId { get; private set; }
    public HairStyle HairStyle { get; private set; }
    public HairColorType HairColorType { get; private set; }
    public EyeColorType EyeColorType { get; private set; }
    public BodyColorType BodyColorType { get; private set; }

    public SpriteLayerState<CharacterSpriteLayer> State { get; set; }
    public Sprite PortraitSprite { get; set; }
    public PortraitSize PortraitSize { get; private set; }

    #endregion

    #region Initialization
    public CharacterComponentAppearance(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public async void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        PortraitSpriteId = characterData.CharacterId;
        HairStyle = characterData.HairStyle;
        HairColorType = characterData.HairColorType;
        EyeColorType = characterData.EyeColorType;
        BodyColorType = characterData.BodyColorType;
        PortraitSize = characterData.PortraitSize;

        if (characterSaveData != null && characterSaveData.IsCustomAvatar) 
        {
            PortraitSpriteId = null;
            HairStyle = characterSaveData.CustomHairStyle;
            HairColorType = characterSaveData.CustomHairColorType;
            EyeColorType = characterSaveData.CustomEyeColorType;
            BodyColorType = characterSaveData.CustomBodyColorType;
            PortraitSize = characterSaveData.CustomPortraitSize;
        }

        State = new SpriteLayerState<CharacterSpriteLayer>();

        ApplyColors();
        await LoadAsync();
    }

    public CharacterComponentAppearance(NpcData npcData)
    {
        Initialize(npcData);
    }

    public async void Initialize(NpcData data)
    {
        PortraitSpriteId = null;
        HairStyle = data.HairStyle;
        HairColorType = data.HairColorType;
        EyeColorType = data.EyeColorType;
        BodyColorType = data.BodyColorType;
        PortraitSize = data.PortraitSize;

        State = new SpriteLayerState<CharacterSpriteLayer>();

        ApplyColors();
        PortraitSprite = data.PortraitSprite;
        await LoadAsync();
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {
        await LoadPortraitSprite();
    }

    private async Task LoadPortraitSprite()
    {
        if (PortraitSpriteId == null) return;
        PortraitSprite = await SpriteAtlasManager.Instance.GetCharacterPortrait(PortraitSpriteId);
    }

    #endregion

    #region Appearance Application

    private void ApplyColors()
    {
        State.Colors[CharacterSpriteLayer.Hair] = ColorManager.GetHairColor(HairColorType);
        State.Colors[CharacterSpriteLayer.EyeIris] = ColorManager.GetEyeColor(EyeColorType);
        State.Colors[CharacterSpriteLayer.Body] = ColorManager.GetBodyColor(BodyColorType);

        //prevent missing key
        State.Colors[CharacterSpriteLayer.KitBase] = Color.black;
        State.Colors[CharacterSpriteLayer.KitDetail] = Color.black;
        State.Colors[CharacterSpriteLayer.KitShocks] = Color.black;
    }

    public void ApplyKit(Kit kit, Variant variant, Position position)
    {
        ApplyKit(kit, variant, GetKitRole(position));
    }

    public void ApplyKit(Kit kit, Variant variant, Role role)
    {
        var kitColor = kit.GetColors(variant, role);

        State.Colors[CharacterSpriteLayer.KitBase] = kitColor.Base;
        State.Colors[CharacterSpriteLayer.KitDetail] = kitColor.Detail;
        State.Colors[CharacterSpriteLayer.KitShocks] = kitColor.Shocks;
    }

    #endregion

    #region Visibility

    public void InitializeVisibility()
    {
        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            State.VisibleLayers.Add(layer);

        State.VisibleLayers.Remove(CharacterSpriteLayer.Aura);
        State.VisibleLayers.Remove(CharacterSpriteLayer.Armor);
    }

    #endregion

    #region Helpers

    public Variant GetKitVariant(Team team) => team?.Variant ?? Variant.Home;
    public Role GetKitRole(Position position) => position == Position.GK ? Role.Keeper : Role.Field;

    #endregion
}
