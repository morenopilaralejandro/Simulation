using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.World;

/// <summary>
/// The overworld is a flat collection of ALL chunks across every zone.
/// This is the single source of truth for what can be streamed.
/// </summary>
[CreateAssetMenu(fileName = "OverworldDefinition", menuName = "ScriptableObject/World/OverworldDefinition")]
public class OverworldDefinition : ScriptableObject
{
    public string OverworldId;
    public Realm Realm;

    [Header("All Overworld Chunks (across all zones)")]
    public List<ChunkDefinition> allChunks = new List<ChunkDefinition>();

    [Header("All Zones (for lookup / UI / spawn resolution)")]
    public List<ZoneDefinition> allZones = new List<ZoneDefinition>();

    [Header("Starting Zone")]
    public ZoneDefinition startingZone;
    public string startingSpawnId;

    [Header("Streaming")]
    [Tooltip("Chunk load radius in chunk coordinates")]
    public int chunkLoadRadius = 2;

}
