using UnityEngine;
using System;
using System.Collections.Generic;

public class SpriteLayerRenderer<TEnum> where TEnum : Enum
{
    private readonly Dictionary<TEnum, SpriteRenderer> lookup;

    public SpriteLayerRenderer(IEnumerable<LayerRenderer<TEnum>> renderers)
    {
        lookup = new Dictionary<TEnum, SpriteRenderer>();
        foreach (var r in renderers)
            lookup[r.layer] = r.renderer;
    }

    public void SetSprite(TEnum layer, Sprite sprite)
    {
        if (lookup.TryGetValue(layer, out var r))
            r.sprite = sprite;
    }

    public void SetColor(TEnum layer, Color color)
    {
        if (lookup.TryGetValue(layer, out var r))
            r.color = color;
    }

    public void SetVisible(TEnum layer, bool visible)
    {
        if (lookup.TryGetValue(layer, out var r))
            r.enabled = visible;
    }

    public void SetActive(TEnum layer, bool active)
    {
        if (lookup.TryGetValue(layer, out var r))
            r.gameObject.SetActive(active);
    }
}
