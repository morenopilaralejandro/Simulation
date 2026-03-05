// PlayerMovementController.cs
using UnityEngine;
using Simulation.Enums.World;

public class PlayerWorldComponentModel : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    private FacingDirection facingDirection = FacingDirection.Down;
    public FacingDirection FacingDirection => facingDirection;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
    }

    public void SetFacing(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            facingDirection = input.x > 0 ? FacingDirection.Right : FacingDirection.Left;
        else
            facingDirection = input.y > 0 ? FacingDirection.Up : FacingDirection.Down;
    }

    public void SetFacing(FacingDirection dir)
    {
        facingDirection = dir;
        //UpdateAnimation();
    }

    public Vector3 VectorToFacing()
    {
        return facingDirection switch
        {
            FacingDirection.Up    => Vector3.forward,
            FacingDirection.Down  => Vector3.back,
            FacingDirection.Left  => Vector3.left,
            FacingDirection.Right => Vector3.right,
            _                     => Vector3.forward
        };
    }

    public Vector2 FacingToVector(FacingDirection dir) => dir switch
    {
        FacingDirection.Up    => Vector2.up,
        FacingDirection.Down  => Vector2.down,
        FacingDirection.Left  => Vector2.left,
        FacingDirection.Right => Vector2.right,
        _                     => Vector2.down
    };

}
