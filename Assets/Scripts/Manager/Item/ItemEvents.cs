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

    public static event Action<CurrencyType, int> OnCurrencyUpdated;
    public static void RaiseCurrencyUpdated(CurrencyType currencyType, int intValue)
    {
        OnCurrencyUpdated?.Invoke(currencyType, intValue);
    }
}
