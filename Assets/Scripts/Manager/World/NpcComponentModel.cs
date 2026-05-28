using UnityEngine;
using Aremoreno.Enums.World;
using Aremoreno.Enums.Animation;

public class NpcComponentModel : MonoBehaviour
{
    private NpcEntity npcEntity;

    private CharacterDirection facingDirection = CharacterDirection.Down;
    public CharacterDirection FacingDirection => facingDirection;

    public void Initialize(NpcEntity npcEntity)
    {
        this.npcEntity = npcEntity;
    }

    public void SetFacing(CharacterDirection dir)
    {
        facingDirection = dir;
        //UpdateAnimation();
    }
}
