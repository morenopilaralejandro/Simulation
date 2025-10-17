using UnityEngine;

public class BallComponentAppearance : MonoBehaviour
{
    #region Renderer
    [SerializeField] private Renderer ballRenderer;    //inspector
    #endregion

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ballRenderer.material = ballData.BallMaterial;
    }

    private void OnDestroy()
    {

    }

}
