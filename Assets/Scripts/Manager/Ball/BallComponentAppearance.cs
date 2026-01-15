using UnityEngine;

public class BallComponentAppearance : MonoBehaviour
{
    #region Renderer
    [SerializeField] private Renderer ballRenderer;    //inspector
    private MaterialPropertyBlock propertyBlock;
    #endregion

    public void Initialize(BallData ballData, Ball ball)
    {
        propertyBlock = new MaterialPropertyBlock();

        ballRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", ballData.Texture);
        ballRenderer.SetPropertyBlock(propertyBlock);
    }

    private void OnDestroy()
    {

    }

}
