using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.World;

public class FastTravelPointComponentAttributes
{
    public string FastTravelPointId { get; private set; }
    public string FlagId { get; private set; }
    public string ZoneId { get; private set; }
    public string SpawnPointId { get; private set; }

    public FastTravelPointComponentAttributes(FastTravelPointData data)
    {
        FastTravelPointId = data.FastTravelPointId;
        FlagId = data.FlagId;
        ZoneId = data.ZoneId;
        SpawnPointId = data.SpawnPointId;
    }
}
