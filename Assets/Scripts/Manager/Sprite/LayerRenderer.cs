using UnityEngine;
using System;
using Simulation.Enums.SpriteLayer;

[Serializable]
public struct LayerRenderer<TEnum> where TEnum : Enum
{
    public TEnum layer;
    public SpriteRenderer renderer;
}
