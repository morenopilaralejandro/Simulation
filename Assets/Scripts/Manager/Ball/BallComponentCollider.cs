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

        // keep vs keeper
        CharacterEntityBattle character = null;
        bool isCharacterKeepCollision = false;
        bool isCharacterKeeperCollision = false;
        bool isValidCollision = false;

        if (otherCollider.CompareTag(tagCharacterKeep)) 
        {
            character = otherCollider.GetComponent<CharacterComponentColliderKeep>().CharacterEntityBattle;
            isCharacterKeepCollision = true;
        }
        else if (otherCollider.CompareTag(tagCharacterKeeper)) 
        {
            character = otherCollider.GetComponent<CharacterComponentColliderDuelKeeper>().CharacterEntityBattle;
            isCharacterKeeperCollision = IsCharacterKeeperCollision(character, otherCollider);
        }

        isValidCollision = isCharacterKeepCollision || isCharacterKeeperCollision;

        if (!isValidCollision) return;

        if (character.CanGainBall())
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
            ball.CancelTravel();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject hitObj = collision.collider.gameObject;
        if (ball.IsTraveling && hitObj.CompareTag(tagBound))
        {
            LogManager.Trace($"[BallComponentCollider] [OnCollisionEnter] {hitObj.name} (Tag: {hitObj.tag}", this);
            ball.CancelTravel();
        }
    }

    private bool IsCharacterKeeperCollision(
        CharacterEntityBattle characterEntityBattle, 
        Collider otherCollider) =>
        //keeper won't stop a pass from a player in its same team
        characterEntityBattle.IsKeeper &&
        characterEntityBattle.IsInOwnPenaltyArea() &&
        PossessionManager.Instance.LastCharacter &&
        !PossessionManager.Instance.LastCharacter.IsSameTeam(characterEntityBattle);

}
