using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ChunkDefinition
{
    public string chunkId;
    public ZoneDefinition parentZone;
    public Vector2Int chunkCoord;
    public string sceneAddress;
    public List<string> containedSpawnIds = new List<string>();

    // No more manual Bounds field — compute it
    public Bounds GetWorldBounds()
    {
        Vector3 center = new Vector3(
            (chunkCoord.x + 0.5f) * WorldConstants.CHUNK_SIZE,
            (chunkCoord.y + 0.5f) * WorldConstants.CHUNK_SIZE,
            0f
        );
        Vector3 size = new Vector3(
            WorldConstants.CHUNK_SIZE,
            WorldConstants.CHUNK_SIZE,
            1f
        );
        return new Bounds(center, size);
    }
}
