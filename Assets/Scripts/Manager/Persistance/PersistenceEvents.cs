using System;

public static class PersistenceEvents
{
    public static event Action<SaveData> OnGameSaved;
    public static void RaiseGameSaved(SaveData saveData)
    {
        OnGameSaved?.Invoke(saveData);
    }

    public static event Action<SaveData> OnGameLoaded;
    public static void RaiseGameLoaded(SaveData saveData)
    {
        OnGameLoaded?.Invoke(saveData);
    }
}
