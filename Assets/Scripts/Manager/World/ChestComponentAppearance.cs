using UnityEngine;
using System;
using System.Threading.Tasks;
using Aremoreno.Enums.World;

public class ChestComponentAppearance : MonoBehaviour
{
    #region Fields

    private ChestEntity chestEntity;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteClosed;
    [SerializeField] private Sprite spriteOpened;

    #endregion

    #region Initialization

    public void Initialize(ChestEntity chestEntity)
    {
        this.chestEntity = chestEntity;
    }

    #endregion

    #region Async Loading

    /*

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

    */

    #endregion

    #region Helpers

    public void SetSpriteOpened() 
    {
        spriteRenderer.sprite = spriteOpened;
    }

    public void SetSpriteClosed() 
    {
        spriteRenderer.sprite = spriteClosed;
    }

    #endregion
}
