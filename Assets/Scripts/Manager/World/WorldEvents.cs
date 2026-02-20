using System;
using Simulation.Enums.World;

public static class WorldEvents
{
    public static event Action OnZoneChanged;
    public static void RaiseZoneChanged()
    {
        OnZoneChanged?.Invoke();
    }
}
