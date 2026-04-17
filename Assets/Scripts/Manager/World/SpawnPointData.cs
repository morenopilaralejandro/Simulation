using UnityEngine;
using Aremoreno.Enums.World;

[System.Serializable]
public class SpawnPointData
{
    public string spawnId;
    public SpawnPointType type;
    public Vector3 position;
    public Vector2 facingDirection;
    public string connectedZoneId;
    public string connectedSpawnId;
}
