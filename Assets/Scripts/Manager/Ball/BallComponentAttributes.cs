using UnityEngine;

public class BallComponentAttributes : MonoBehaviour
{
    [SerializeField] private string ballId;

    public string BallId => ballId;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ballId = ballData.BallId;
    }
}
