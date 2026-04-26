using UnityEngine;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.World;

public class NpcComponentInteractable : Interactable
{
    private NpcEntity npcEntity;

    public virtual void Initialize(NpcEntity npcEntity)
    {
        this.npcEntity = npcEntity;
    }

    protected override void InteractInternal()
    {
        LogManager.Error("NPC Interacted!");
    }

}
