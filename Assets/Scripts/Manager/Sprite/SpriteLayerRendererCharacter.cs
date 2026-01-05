using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.SpriteLayer;

public class SpriteLayerRendererCharacter : MonoBehaviour
{
    [SerializeField] private LayerRenderer<CharacterSpriteLayer>[] renderers;

    private SpriteLayerRenderer<CharacterSpriteLayer> spriteLayerRenderer;

    private void Awake()
    {
        spriteLayerRenderer = new SpriteLayerRenderer<CharacterSpriteLayer>(renderers);
    }


    public void SetSprite(CharacterSpriteLayer layer, Sprite sprite)
        => spriteLayerRenderer.SetSprite(layer, sprite);

    public void SetColor(CharacterSpriteLayer layer, Color color)
        => spriteLayerRenderer.SetColor(layer, color);

    public void SetVisible(CharacterSpriteLayer layer, bool visible)
        => spriteLayerRenderer.SetVisible(layer, visible);

    public void SetActive(CharacterSpriteLayer layer, bool active)
        => spriteLayerRenderer.SetActive(layer, active);
}
