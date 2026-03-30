using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.World;

public static class WorldArgs
{
    public static string ZoneId;
    public static Realm Realm;
    public static Vector3 PlayerPosition;
    public static Vector2 FacingDirection;
    public static WorldState WorldState;

    public static void Clear()
    {
        WorldState = WorldState.None;
    }

    public static void Set(
        string zoneId,
        Realm realm,
        Vector3 playerPosition,
        Vector2 facingDirection,
        WorldState worldState)
    {
        ZoneId = zoneId;
        Realm = realm;
        PlayerPosition = playerPosition;
        FacingDirection = facingDirection;
        WorldState = worldState;
    }
}
