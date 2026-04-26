using UnityEngine;
using System;
using System.Threading.Tasks;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.SpriteLayer;

public class CharacterComponentAppearanceBattle : MonoBehaviour, IAsyncSceneLoader
{
    #region Fields

    [SerializeField] private SpriteLayerRendererCharacter spriteLayerRenderer;

    private CharacterEntityBattle characterEntityBattle;
    private bool spritesLoaded;

    #endregion

    #region Initialization

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {
        await LoadSprites();

        characterEntityBattle.InitializeVisibility();
        ToggleGloves(characterEntityBattle.FormationCoord.Position);
        ApplyStateToRenderer();
    }

    private async Task LoadSprites()
    {
        characterEntityBattle.SpriteLayerState.Sprites[CharacterSpriteLayer.Hair] =
            await SpriteAtlasManager.Instance.GetCharacterHair(characterEntityBattle.HairStyle.ToString().ToLower());

        spritesLoaded = true;
    }

    #endregion

    #region Appearance Application

    public void ApplyStateToRenderer()
    {
        if (!spritesLoaded) return;

        foreach (var (layer, sprite) in characterEntityBattle.SpriteLayerState.Sprites)
            spriteLayerRenderer.SetSprite(layer, sprite);

        foreach (var (layer, color) in characterEntityBattle.SpriteLayerState.Colors)
            spriteLayerRenderer.SetColor(layer, color);

        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            spriteLayerRenderer.SetVisible(layer, characterEntityBattle.SpriteLayerState.Contains(layer));
    }

    #endregion

    #region Visibility

    public void ToggleGloves(Position position)
    {
        bool isGoalkeeper = position == Position.GK;
        bool hasGloves = characterEntityBattle.SpriteLayerState.VisibleLayers.Contains(CharacterSpriteLayer.Gloves);

        if (isGoalkeeper == hasGloves) return;

        if (isGoalkeeper)
            characterEntityBattle.SpriteLayerState.VisibleLayers.Add(CharacterSpriteLayer.Gloves);
        else
            characterEntityBattle.SpriteLayerState.VisibleLayers.Remove(CharacterSpriteLayer.Gloves);

        spriteLayerRenderer.SetActive(CharacterSpriteLayer.Gloves, isGoalkeeper);
    }

    public void SetCharacterVisible(bool isVisible)
    {
        foreach (CharacterSpriteLayer layer in characterEntityBattle.SpriteLayerState.VisibleLayers)
            spriteLayerRenderer.SetVisible(layer, isVisible);
    }

    #endregion

    #region Helpers

    #endregion
}
