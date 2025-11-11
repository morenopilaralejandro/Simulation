using UnityEngine;
using System.Collections.Generic;

public class FieldLine : MonoBehaviour
{
    [Header("Assign the new material you want to apply")]
    [SerializeField] private Material lineMaterial;

    public void SetMaterial(Material material)
    {
        lineMaterial = material;
        ApplyMaterialToChildren();
    }

    public void ApplyMaterialToChildren()
    {
        if (lineMaterial == null) return;

        // Get all MeshRenderers in this object and its children
        var meshRenderers = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
        foreach (MeshRenderer mr in meshRenderers)
            mr.sharedMaterial = lineMaterial;

        // Get all SpriteRenderers in this object and its children
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        foreach (SpriteRenderer sr in spriteRenderers)
            sr.sharedMaterial = lineMaterial;

        LogManager.Trace($"Applied material to renderers under {name}.");
    }
}
