using UnityEngine;
using System.Threading.Tasks;

public class BallComponentAppearance : MonoBehaviour
{
    #region Fields

    [SerializeField] private Renderer ballRenderer;
    private MaterialPropertyBlock propertyBlock;
    private readonly AddressableBinding<Texture2D> _bindingBall = new();

    #endregion

    public void Initialize(BallData ballData, Ball ball)
    {
        propertyBlock = new MaterialPropertyBlock();
        _ = SetTextureAsync(ballRenderer, _bindingBall, ballData.BallTextureAddress);
    }

    private async Task SetTextureAsync(Renderer renderer, AddressableBinding<Texture2D> binding, string address)
    {
        var texture = await binding.LoadAsync(address);

        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_BaseMap", texture);
        renderer.SetPropertyBlock(propertyBlock);
    }

    private void OnDestroy()
    {
        _bindingBall.Release();
        _bindingBall.Cancel();
    }

}
