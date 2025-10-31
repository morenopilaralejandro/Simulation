using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallComponentKinematic : MonoBehaviour
{
    private Ball ball;

    [SerializeField] private Rigidbody ballRigidbody;

    public bool IsKinematic => ballRigidbody.isKinematic;
    public Vector3 GetVelocity() => ballRigidbody.velocity;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    public void SetKinematic()
    {
        /*
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        */
        ballRigidbody.isKinematic = true;
        ballRigidbody.useGravity = false;
    }

    public void SetDynamic()
    {
        ballRigidbody.isKinematic = false;
        ballRigidbody.useGravity = true;
    }

    public void SetDynamic(Vector3 velocity)
    {
        ballRigidbody.isKinematic = false;
        ballRigidbody.velocity = velocity;
        ballRigidbody.useGravity = true;
    }

    public void ToggleKinematic()
    {
        if (ballRigidbody.isKinematic) SetDynamic();
        else SetKinematic();
    }
}
