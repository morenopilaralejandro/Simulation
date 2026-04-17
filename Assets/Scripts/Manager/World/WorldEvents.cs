using System;
using UnityEngine;
using Aremoreno.Enums.World;

public static class WorldEvents
{
    public static event Action<ZoneDefinition, ZoneDefinition, string> OnZoneChanged;
    public static void RaiseZoneChanged(ZoneDefinition previousZone, ZoneDefinition newZone, string newName)
    {
        OnZoneChanged?.Invoke(previousZone, newZone, newName);
    }

    public static event Action<PlayerWorldState, PlayerWorldState> OnPlayerStateChanged;
    public static void RaisePlayerStateChanged(PlayerWorldState playerWorldStateOld, PlayerWorldState playerWorldStateNew)
    {
        OnPlayerStateChanged?.Invoke(playerWorldStateOld, playerWorldStateNew);
    }

    public static event Action<EncounterData> OnEncounterTriggered;
    public static void RaiseEncounterTriggered(EncounterData encounterData)
    {
        OnEncounterTriggered?.Invoke(encounterData);
    }

    public static event Action OnInteractionStarted;
    public static void RaiseInteractionStarted()
    {
        OnInteractionStarted?.Invoke();
    }

    public static event Action OnInteractionEnded;
    public static void RaiseInteractionEnded()
    {
        OnInteractionEnded?.Invoke();
    }

    public static event Action OnMenuOpened;
    public static void RaiseMenuOpened()
    {
        OnMenuOpened?.Invoke();
    }

    public static event Action OnMenuClosed;
    public static void RaiseMenuClosed()
    {
        OnMenuClosed?.Invoke();
    }

    public static event Action<Vector3> OnPlayerTeleported;
    public static void RaisePlayerTeleported(Vector3 position)
    {
        OnPlayerTeleported?.Invoke(position);
    }

    public static event Action<Realm> OnRealmChanged;
    public static void RaiseRealmChanged(Realm realm)
    {
        OnRealmChanged?.Invoke(realm);
    }
}
