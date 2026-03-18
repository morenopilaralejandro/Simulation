using UnityEngine;
using System;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.SpriteLayer;

public class CharacterComponentAppearanceBattle : MonoBehaviour, IAsyncSceneLoader
{
    #region Fields

    [SerializeField] private SpriteLayerRendererCharacter spriteLayerRenderer;

    private CharacterEntityBattle characterEntityBattle;

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

        if (characterEntityBattle.FormationCoord.Position != Position.GK)
        {
            characterEntityBattle.SpriteLayerState.VisibleLayers.Remove(CharacterSpriteLayer.Gloves);
            spriteLayerRenderer.SetActive(CharacterSpriteLayer.Gloves, false);
        }
        characterEntityBattle.InitializeVisibility();
        ApplyStateToRenderer();
    }

    private async Task LoadSprites()
    {
        characterEntityBattle.SpriteLayerState.Sprites[CharacterSpriteLayer.Hair] =
            await SpriteAtlasManager.Instance.GetCharacterHair(characterEntityBattle.HairStyle.ToString().ToLower());

    }

    #endregion

    #region Appearance Application

    private void ApplyStateToRenderer()
    {
        foreach (var (layer, sprite) in characterEntityBattle.SpriteLayerState.Sprites)
            spriteLayerRenderer.SetSprite(layer, sprite);

        foreach (var (layer, color) in characterEntityBattle.SpriteLayerState.Colors)
            spriteLayerRenderer.SetColor(layer, color);

        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            spriteLayerRenderer.SetVisible(layer, characterEntityBattle.SpriteLayerState.Contains(layer));
    }

    #endregion

    #region Visibility

    public void SetCharacterVisible(bool isVisible)
    {
        foreach (CharacterSpriteLayer layer in characterEntityBattle.SpriteLayerState.VisibleLayers)
            spriteLayerRenderer.SetVisible(layer, isVisible);
    }

    #endregion

    #region Helpers


    #endregion
}
