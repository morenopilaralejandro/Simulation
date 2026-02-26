using UnityEngine;
using System;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.SpriteLayer;

public class PlayerWorldAppearanceComponent : MonoBehaviour, IAsyncSceneLoader
{
    #region Fields

    [SerializeField] private SpriteLayerRendererCharacter spriteLayerRenderer;

    private PlayerWorldEntity playerWorldEntity;
    private Character character;

    #endregion

    #region Initialization

    public void Initialize(PlayerWorldEntity playerWorldEntity)
    {
        this.playerWorldEntity = playerWorldEntity;
        this.character = playerWorldEntity.Character;
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {       
        while (character == null) await Task.Yield();
        await LoadSprites();
        character.InitializeVisibility();
        ApplyStateToRenderer();
        playerWorldEntity.MakePersistent();
    }

    private async Task LoadSprites()
    {
        character.SpriteLayerState.Sprites[CharacterSpriteLayer.Hair] =
            await SpriteAtlasManager.Instance.GetCharacterHair(character.HairStyleId);
    }

    #endregion

    #region Appearance Application

    private void ApplyStateToRenderer()
    {
        foreach (var (layer, sprite) in character.SpriteLayerState.Sprites)
            spriteLayerRenderer.SetSprite(layer, sprite);

        foreach (var (layer, color) in character.SpriteLayerState.Colors)
            spriteLayerRenderer.SetColor(layer, color);

        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            spriteLayerRenderer.SetVisible(layer, character.SpriteLayerState.Contains(layer));
    }

    #endregion

    #region Visibility

    #endregion

    #region Helpers

    #endregion
}
