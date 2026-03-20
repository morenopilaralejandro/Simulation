using System;
using Simulation.Enums.Item;

public static class ItemEvents
{
    public static event Action OnStorageUpdated;
    public static void RaiseStorageUpdated()
    {
        OnStorageUpdated?.Invoke();
    }

    public static event Action<Item> OnItemUsed;
    public static void RaiseItemUsed(Item item)
    {
        OnItemUsed?.Invoke(item);
    }
}
