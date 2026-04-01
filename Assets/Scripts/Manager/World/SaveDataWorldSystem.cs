using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.World;

[System.Serializable]
public class SaveDataWorldSystem
{
    // realmSystem;
    public Realm CurrentRealm;
    // zoneSystem;
    // playerSystem;
    public Vector3 PlayerPosition;
    public Vector2 FacingDirection;
    // zoneTrackerSystem;
    public ZoneDefinition CurrentZone;
    // encounterSystem;
}
