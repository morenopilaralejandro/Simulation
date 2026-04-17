using UnityEngine;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.World;

public class ChestComponentInteractable : Interactable
{
    private ChestEntity chestEntity;

    public void Initialize(ChestEntity chestEntity)
    {
        this.chestEntity = chestEntity;
    }

    protected override void InteractInternal()
    {
        chestEntity.StartDialog();
        LogManager.Trace("[ChestComponentInteractable] Chest interacted");
    }

}
