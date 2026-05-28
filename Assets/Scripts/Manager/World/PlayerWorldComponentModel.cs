using UnityEngine;
using Aremoreno.Enums.World;
using Aremoreno.Enums.Animation;

public class PlayerWorldComponentModel : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    private CharacterDirection facingDirection = CharacterDirection.Down;
    public CharacterDirection FacingDirection => facingDirection;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
    }

    public void SetFacing(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            facingDirection = input.x > 0 ? CharacterDirection.Right : CharacterDirection.Left;
        else
            facingDirection = input.y > 0 ? CharacterDirection.Up : CharacterDirection.Down;
    }

    public void SetFacing(CharacterDirection dir)
    {
        facingDirection = dir;
        //UpdateAnimation();
    }

    public Vector3 VectorToFacing()
    {
        return facingDirection switch
        {
            CharacterDirection.Up    => Vector3.forward,
            CharacterDirection.Down  => Vector3.back,
            CharacterDirection.Left  => Vector3.left,
            CharacterDirection.Right => Vector3.right,
            _                     => Vector3.forward
        };
    }

    public Vector2 FacingToVector(CharacterDirection dir) => dir switch
    {
        CharacterDirection.Up    => Vector2.up,
        CharacterDirection.Down  => Vector2.down,
        CharacterDirection.Left  => Vector2.left,
        CharacterDirection.Right => Vector2.right,
        _                     => Vector2.down
    };

    public CharacterDirection GetOppositeFacingDirection() => facingDirection switch
    {
        CharacterDirection.Up    => CharacterDirection.Down,
        CharacterDirection.Down  => CharacterDirection.Up,
        CharacterDirection.Left  => CharacterDirection.Right,
        CharacterDirection.Right => CharacterDirection.Left,
        _                     => CharacterDirection.Down
    };

}
