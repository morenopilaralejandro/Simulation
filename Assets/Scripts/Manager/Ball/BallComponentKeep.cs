using UnityEngine;

public class BallComponentKeep : MonoBehaviour
{
    private Ball ball;
 
    private Character character;
    private float keepSpeed = 30f;
    private float ballY = 0.1f;
    private float ballToPlayerDiscance = 0.2f;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    private void OnEnable()
    {
        BallEvents.OnGained += HandleOnGained;    
    }

    private void OnDisable()
    {
        BallEvents.OnGained -= HandleOnGained;
    }

    private void HandleOnGained(Character character)
    {
        this.character = character;
        ball.SetKinematic();
    }

    void Update() 
    {
        if (this.ball.IsFree() || this.character == null) return;

        Vector3 forwardDir = character.transform.forward;
        Vector3 targetPosition = character.transform.position + forwardDir * ballToPlayerDiscance;
        targetPosition.y = ballY;
        transform.position = Vector3.Lerp(transform.position, targetPosition, keepSpeed * Time.deltaTime);
    }

}
