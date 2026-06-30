using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aremoreno.Enums.Battle;

public class Field : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private FieldLine fieldLineFull;
    [SerializeField] private FieldLine fieldLineMini;
    [SerializeField] private MeshRenderer rendererInner;
    [SerializeField] private MeshRenderer rendererOuter;

    private readonly AddressableBinding<Texture2D> _bindingInner = new();
    private readonly AddressableBinding<Texture2D> _bindingOuter = new();

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

        _bindingInner.Release();
        _bindingOuter.Release();

        _bindingInner.Cancel();
        _bindingOuter.Cancel();
    }

    public void Initialize(FieldData fieldData)
    {
        _ = SetTextureAsync(rendererInner, _bindingInner, fieldData.TextureInnerAddress);
        _ = SetTextureAsync(rendererOuter, _bindingOuter, fieldData.TextureOuterAddress);

        BattleType battleType = BattleManager.Instance.CurrentType;
        if (battleType == BattleType.Full) 
            fieldLineFull.SetColor(ColorManager.GetFieldLineColor(fieldData.FieldLineColor));
        else
            fieldLineMini.SetColor(ColorManager.GetFieldLineColor(fieldData.FieldLineColor));
    }

    private async Task SetTextureAsync(MeshRenderer renderer, AddressableBinding<Texture2D> binding, string address)
    {
        var texture = await binding.LoadAsync(address);

        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_BaseMap", texture);
        renderer.SetPropertyBlock(propertyBlock);
    }
}
