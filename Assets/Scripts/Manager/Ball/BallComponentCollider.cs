using UnityEngine;

public class BallComponentCollider : MonoBehaviour
{
    private Ball ball;
    private string tagCharacterKeep = "Character-Keep";
    private string tagCharacterKeeper = "Character-Duel-Keeper";
    private string tagBound = "Bound";
    
    
    [SerializeField] private Collider ballCollider;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    private void OnTriggerEnter(Collider otherCollider) 
    {
        HandleTrigger(otherCollider);
    }

    private void OnTriggerStay(Collider otherCollider) 
    {
        HandleTrigger(otherCollider);
    }

    private void HandleTrigger(Collider otherCollider)
    {
        //travel collision handle in player colliders
        if (ball.IsTraveling) return;

        Character character = otherCollider.GetComponentInParent<Character>();

        if (character == null) return;

        bool isCharacterKeeperCollision = IsCharacterKeeperCollision(character, otherCollider);
      
        if (
            character.CanGainBall() &&
            (IsCharacterKeepCollision(character, otherCollider) || isCharacterKeeperCollision)
        )
        {
            LogManager.Trace($"[BallComponentCollider] [OnTriggerEnter] {character.CharacterId}", this);
            PossessionManager.Instance.Gain(character);
            if (isCharacterKeeperCollision) 
            {
                //AudioManager.Instance.PlaySfx("SfxCatch");
            }
                
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObj = collision.collider.gameObject;
        if (ball.IsTraveling && hitObj.CompareTag(tagBound))
        {
            LogManager.Trace($"[BallComponentCollider] [OnCollisionEnter] {hitObj.name} (Tag: {hitObj.tag}", this);
            //DuelLogManager.Instance.AddDuelCancel();
            //BallTravelController.Instance.CancelTravel();
        }
    }

    private bool IsCharacterKeepCollision(
        Character character, 
        Collider otherCollider) =>
        otherCollider.CompareTag(tagCharacterKeep);

    private bool IsCharacterKeeperCollision(
        Character character, 
        Collider otherCollider) =>
        //keeper won't stop a pass from a player in its same team
        character.IsKeeper &&
        character.IsInOwnPenaltyArea() &&
        otherCollider.CompareTag(tagCharacterKeeper) &&
        PossessionManager.Instance.LastCharacter &&
        !PossessionManager.Instance.LastCharacter.IsSameTeam(character);

}
