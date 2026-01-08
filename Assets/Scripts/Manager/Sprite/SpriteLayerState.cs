using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerState<TEnum> where TEnum : Enum
{
    public Dictionary<TEnum, Sprite> Sprites = new();
    public Dictionary<TEnum, Color> Colors = new();
    public HashSet<TEnum> VisibleLayers = new();

    public void Clear()
    {
        Sprites.Clear();
        Colors.Clear();
        VisibleLayers.Clear();
    }

    public bool Contains(TEnum layer)
        => VisibleLayers.Contains(layer);
}
