using UnityEngine;
using System;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.SpriteLayer;

public class CharacterComponentAppearance : MonoBehaviour, IAsyncSceneLoader
{
    #region Serialized Fields

    [SerializeField] private SpriteLayerRendererCharacter spriteLayerRenderer;

    #endregion

    #region Private Fields

    private Character character;
    private CharacterData characterData;
    private SpriteLayerState<CharacterSpriteLayer> state;
    private Sprite portraitSprite;
    private PortraitSize portraitSize;

    #endregion

    #region Public Properties

    public SpriteLayerState<CharacterSpriteLayer> SpriteLayerState => state;
    public Sprite PortraitSprite => portraitSprite;
    public PortraitSize PortraitSize => portraitSize;

    #endregion

    #region Initialization

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
        this.characterData = characterData;

        state = new SpriteLayerState<CharacterSpriteLayer>();
        portraitSize = characterData.PortraitSize;

        InitializeVisibility();
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {
        await LoadSprites();

        ApplyColors(characterData);

        if (character.FormationCoord.Position != Position.GK)
        {
            state.VisibleLayers.Remove(CharacterSpriteLayer.Gloves);
            spriteLayerRenderer.SetActive(CharacterSpriteLayer.Gloves, false);
        }

        ApplyKit();
        ApplyStateToRenderer();
    }

    private async Task LoadSprites()
    {
        state.Sprites[CharacterSpriteLayer.Hair] =
            await SpriteAtlasManager.Instance.GetCharacterHair(
                characterData.HairStyle.ToString().ToLower());

        portraitSprite =
            await SpriteAtlasManager.Instance.GetCharacterPortrait(
                characterData.CharacterId);
    }

    #endregion

    #region Appearance Application

    private void ApplyColors(CharacterData characterData)
    {
        var hairColor = ColorManager.GetHairColor(characterData.HairColorType);

        state.Colors[CharacterSpriteLayer.Hair] = hairColor;
        state.Colors[CharacterSpriteLayer.EyeIris] = ColorManager.GetEyeColor(characterData.EyeColorType);
        state.Colors[CharacterSpriteLayer.Body] = ColorManager.GetBodyColor(characterData.BodyColorType);
    }

    public void ApplyKit()
    {
        var kitColor = character.GetTeam().Kit.GetColors(GetKitVariant(), GetKitRole());

        state.Colors[CharacterSpriteLayer.KitBase] = kitColor.Base;
        state.Colors[CharacterSpriteLayer.KitDetail] = kitColor.Detail;
        state.Colors[CharacterSpriteLayer.KitShocks] = kitColor.Shocks;
    }

    private void ApplyStateToRenderer()
    {
        foreach (var (layer, sprite) in state.Sprites)
            spriteLayerRenderer.SetSprite(layer, sprite);

        foreach (var (layer, color) in state.Colors)
            spriteLayerRenderer.SetColor(layer, color);

        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            spriteLayerRenderer.SetVisible(layer, state.Contains(layer));
    }

    #endregion

    #region Visibility

    private void InitializeVisibility()
    {
        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            state.VisibleLayers.Add(layer);

        state.VisibleLayers.Remove(CharacterSpriteLayer.Aura);
        state.VisibleLayers.Remove(CharacterSpriteLayer.Armor);
    }

    public void SetCharacterVisible(bool isVisible)
    {
        foreach (CharacterSpriteLayer layer in state.VisibleLayers)
            spriteLayerRenderer.SetVisible(layer, isVisible);
    }

    #endregion

    #region Kit Helpers

    public Role GetKitRole()
    {
        return character.FormationCoord.Position == Position.GK ? Role.Keeper : Role.Field;
    }

    public Variant GetKitVariant()
    {
        return character.GetTeam().Variant;
    }

    #endregion
}
