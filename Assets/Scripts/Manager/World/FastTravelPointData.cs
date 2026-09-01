using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.World;

[CreateAssetMenu(fileName = "FastTravelPointData", menuName = "ScriptableObject/World/FastTravelPointData")]
public class FastTravelPointData : ScriptableObject
{
    public string FastTravelPointId;
    public string FlagId;
    public string ZoneId;
    public string SpawnPointId;
}
