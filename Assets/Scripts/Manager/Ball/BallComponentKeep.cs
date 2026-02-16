using UnityEngine;

public class BallComponentKeep : MonoBehaviour
{
    private Ball ball;
 
    private Character character;
    private CharacterEntityBattle characterEntityBattle;
    private float keepSpeed = 30f;
    private float ballYDefault = 0.1f;
    private float ballYHand = 0.4f;
    private float ballToPlayerDiscance = 0.2f;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    private void OnEnable()
    {
        BallEvents.OnGained += HandleOnGained;
        BallEvents.OnReleased += HandleOnReleased;
    }

    private void OnDisable()
    {
        BallEvents.OnGained -= HandleOnGained;
        BallEvents.OnReleased -= HandleOnReleased;
    }

    private void HandleOnGained(CharacterEntityBattle characterEntityBattle)
    {
        this.character = characterEntityBattle.Character;
        this.characterEntityBattle = characterEntityBattle;
        characterEntityBattle.StartControl();
        ball.SetKinematic();
    }

    private void HandleOnReleased(CharacterEntityBattle characterEntityBattle)
    {
        if (!ball.IsTraveling)
            ball.SetDynamic();
    }

    void Update() 
    {
        if (ball.IsFree() || character == null) return;

        Vector3 forwardDir = characterEntityBattle.Model.transform.forward;
        Vector3 targetPosition = characterEntityBattle.transform.position + forwardDir * ballToPlayerDiscance;
        targetPosition.y = characterEntityBattle.HasBallInHand || characterEntityBattle.HasBallInHandThrowIn ? ballYHand : ballYDefault;
        transform.position = Vector3.Lerp(transform.position, targetPosition, keepSpeed * Time.deltaTime);
    }

}
