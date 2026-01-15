using UnityEngine;
using System.Collections.Generic;

public class Field : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private FieldLine fieldLine;
    [SerializeField] private MeshRenderer rendererInner;
    [SerializeField] private MeshRenderer rendererOuter;

    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    void Start() 
    {
        BattleFieldManager.Instance.RegisterField(this);
    }

    void Destroy() 
    {
        BattleFieldManager.Instance.UnregisterField();
    }

    public void Initialize(FieldData fieldData)
    {
        rendererInner.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", fieldData.TextureInner);
        rendererInner.SetPropertyBlock(propertyBlock);

        rendererOuter.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", fieldData.TextureOuter);
        rendererOuter.SetPropertyBlock(propertyBlock);

        fieldLine.SetColor(fieldData.LineColor);
    }

}
