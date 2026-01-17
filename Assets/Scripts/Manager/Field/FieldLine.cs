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

    public void SetColor(Color color)
    {
        lineColor = color;
        ApplyToChildren();
    }

    private void ApplyToChildren()
    {
        //mesh
        var meshRenderers = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
        meshRenderers[0].GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", lineColor);

        foreach (var mr in meshRenderers) 
            mr.SetPropertyBlock(propertyBlock);

        //sprite
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        foreach (var sr in spriteRenderers)
            sr.color = lineColor;

        LogManager.Trace($"[FieldLine] Updated renderers under {name}.");
    }
}
