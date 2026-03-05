using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.World;

public class ChestComponentInteractable : Interactable
{
    private ChestEntity chestEntity;
    private bool opened;

    public void Initialize(ChestEntity chestEntity)
    {
        this.chestEntity = chestEntity;
    }

    protected override void InteractInternal()
    {
        if (opened) return;
        opened = true;
        LogManager.Error("Chest Opened!");
    }

}
