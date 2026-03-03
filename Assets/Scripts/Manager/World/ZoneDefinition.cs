using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.World;

[CreateAssetMenu(fileName = "ZoneDefinition", menuName = "ScriptableObject/World/ZoneDefinition")]
public class ZoneDefinition : ScriptableObject
{
    [Header("Zone Info")]
    public string zoneId;
    public string zoneName;
    public ZoneType zoneType;

    [Header("Interior Scene (only for Interior zones)")]
    public string interiorSceneAddress;

    [Header("Audio")]
    public AudioClip backgroundMusic;
}

