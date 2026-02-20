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

    [Header("Overworld Chunks (only for Overworld zones)")]
    public List<ChunkDefinition> chunks = new List<ChunkDefinition>();

    [Header("Interior Scene (only for Interior zones)")]
    public string interiorSceneAddress;

    [Header("Zone Settings")]
    public int chunkLoadRadius = 1; // How many chunks around player to load
    public string defaultSpawnId;

    [Header("Audio")]
    public AudioClip backgroundMusic;
}

