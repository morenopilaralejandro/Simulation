using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.World;

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
