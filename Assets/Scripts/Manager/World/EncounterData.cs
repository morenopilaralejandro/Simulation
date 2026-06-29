using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.World;

[System.Serializable]
public class EncounterData
{
    public string TeamId;
    public int Level;
    public string BgmId;
    public string BallId;

    [Range(0f, 1f)]
    public float EncounterRate = 0.5f;

    public bool HasTimeOfDayRestriction;
    public TimeOfDay TimeOfDay;

    public List<ItemDrop> Drops = new List<ItemDrop>();
}
