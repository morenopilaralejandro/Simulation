using UnityEngine;
using System.Collections.Generic;

public class FieldLine : MonoBehaviour
{
    private Color lineColor;
    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        ApplyToChildren();
    }

    void Start()
    {
        ApplyToChildren();
    }

    public void SetColor(Color color)
    {
        lineColor = color;
        ApplyToChildren();
    }


    private void ApplyToChildren()
    {
        // Mesh renderers with MaterialPropertyBlock
        var meshRenderers = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
        
        foreach (var mr in meshRenderers)
        {
            mr.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", lineColor);
            mr.SetPropertyBlock(propertyBlock);
        }

        // Sprite renderers with MaterialPropertyBlock
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        
        foreach (var sr in spriteRenderers)
        {
            sr.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", lineColor);
            sr.SetPropertyBlock(propertyBlock);
        }

        LogManager.Trace($"[FieldLine] Updated {meshRenderers.Length} mesh + {spriteRenderers.Length} sprite renderers under {name}.");
    }
}
