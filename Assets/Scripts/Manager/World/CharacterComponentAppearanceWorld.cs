using UnityEngine;
using System;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.SpriteLayer;

public class CharacterComponentAppearanceWorld : MonoBehaviour, IAsyncSceneLoader
{
    #region Fields

    [SerializeField] private SpriteLayerRendererCharacter spriteLayerRenderer;

    private CharacterComponentAppearance appearanceComponent;
    private PlayerWorldEntity playerWorldEntity;

    #endregion

    #region Initialization

    public void Initialize(CharacterComponentAppearance appearanceComponent, PlayerWorldEntity playerWorldEntity = null)
    {
        this.appearanceComponent = appearanceComponent;
        this.playerWorldEntity = playerWorldEntity;
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {
        while (appearanceComponent == null) await Task.Yield();
        await LoadSprites();
        appearanceComponent.InitializeVisibility();
        ApplyStateToRenderer();
        if (playerWorldEntity != null)
            playerWorldEntity.MakePersistent();
    }

    private async Task LoadSprites()
    {
        appearanceComponent.State.Sprites[CharacterSpriteLayer.Hair] =
            await SpriteAtlasManager.Instance.GetCharacterHairWorld(appearanceComponent.HairStyleId);
    }

    #endregion

    #region Appearance Application

    private void ApplyStateToRenderer()
    {
        foreach (var (layer, sprite) in appearanceComponent.State.Sprites)
            spriteLayerRenderer.SetSprite(layer, sprite);

        foreach (var (layer, color) in appearanceComponent.State.Colors)
            spriteLayerRenderer.SetColor(layer, color);

        foreach (CharacterSpriteLayer layer in Enum.GetValues(typeof(CharacterSpriteLayer)))
            spriteLayerRenderer.SetVisible(layer, appearanceComponent.State.Contains(layer));
    }

    #endregion

}
