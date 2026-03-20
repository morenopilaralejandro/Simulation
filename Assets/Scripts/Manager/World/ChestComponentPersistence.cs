using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.World;

public class ChestComponentPersistence
{
    private Chest chest;
    private ChestStateManager chestStateManager;

    public bool IsPersistent { get; private set; }

    public ChestComponentPersistence(Chest chest, bool isPersistent)
    {
        Initialize(chest, isPersistent);
    }

    public void Initialize(Chest chest, bool isPersistent)
    {
        this.Chest = chest;
        chestStateManager = ChestStateManager.Instance;
        IsPersistent = isPersistent;
    }

    public bool IsOpenedPersistent => IsPersistent && chestStateManager.IsOpened(chest.ChestId);
    public void OpenPersistent() => chestStateManager.Open(chest.ChestId);
}
