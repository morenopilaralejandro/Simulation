using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;

public class Field : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private FieldLine fieldLineFull;
    [SerializeField] private FieldLine fieldLineMini;
    [SerializeField] private MeshRenderer rendererInner;
    [SerializeField] private MeshRenderer rendererOuter;

    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    void Start() 
    {
        BattleManager.Instance.RegisterField(this);
    }

    void Destroy() 
    {
        BattleManager.Instance.UnregisterField();
    }

    public void Initialize(FieldData fieldData)
    {
        rendererInner.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", fieldData.TextureInner);
        rendererInner.SetPropertyBlock(propertyBlock);

        rendererOuter.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", fieldData.TextureOuter);
        rendererOuter.SetPropertyBlock(propertyBlock);

        BattleType battleType = BattleManager.Instance.CurrentType;
        if (battleType == BattleType.Full) 
        {
            fieldLineFull.SetColor(fieldData.LineColor);
        } else
        {
            fieldLineMini.SetColor(fieldData.LineColor);
        }
    }

}
