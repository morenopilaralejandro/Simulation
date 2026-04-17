using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.World;

[System.Serializable]
public class SaveDataWorldSystem
{
    // realmSystem;
    public Realm Realm;
    // zoneSystem;
    // playerSystem;
    public Vector3 PlayerPosition;
    public Vector2 FacingDirection;
    // zoneTrackerSystem;
    public string ZoneId;
    // encounterSystem;
}
