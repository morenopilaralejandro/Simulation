using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Localization;

/// <summary>
/// Tracks which zone the player is currently in based on loaded chunks
/// or zone bounds. Fires events on zone transitions.
/// </summary>
public class WorldManagerZoneTracker
{
    #region Fields

    public ZoneDefinition CurrentZone { get; private set; }
    public ZoneDefinition PreviousZone { get; private set; }

    // O(1) chunk coord → zone lookup built at init
    private readonly Dictionary<Vector2Int, ZoneDefinition> _chunkCoordToZone
        = new Dictionary<Vector2Int, ZoneDefinition>();
    private float _invChunkSize;
    private OverworldDefinition _overworld;

    private LocalizationComponentString localizationStringComponent;
    public string ZoneName => localizationStringComponent.GetString(LocalizationField.Name);

    #endregion

    #region Constructor

    /// <summary>
    /// Build the coord-to-zone map from the overworld definition.
    /// Call once when streaming starts.
    /// </summary>
    public WorldManagerZoneTracker(OverworldDefinition overworld)
    {
        _overworld = overworld;
        _invChunkSize = 1f / WorldConstants.CHUNK_SIZE;
        _chunkCoordToZone.Clear();

        List<ChunkDefinition> chunks = overworld.allChunks;
        for (int i = 0, len = chunks.Count; i < len; i++)
        {
            ChunkDefinition chunk = chunks[i];
            if (chunk.parentZone != null)
            {
                // First chunk to claim a coord wins; overlapping zones
                // would need a priority system (out of scope here)
                if (!_chunkCoordToZone.ContainsKey(chunk.chunkCoord))
                {
                    _chunkCoordToZone[chunk.chunkCoord] = chunk.parentZone;
                }
            }
        }

    }

    #endregion

    #region Logic Positional

    /// <summary>
    /// Call every time the player moves to a new chunk coord (or periodically).
    /// </summary>
    public void UpdatePlayerPosition(Vector3 worldPos)
    {
        int cx = Mathf.FloorToInt(worldPos.x * _invChunkSize);
        int cy = Mathf.FloorToInt(worldPos.y * _invChunkSize);
        Vector2Int coord = new Vector2Int(cx, cy);

        ZoneDefinition newZone;
        _chunkCoordToZone.TryGetValue(coord, out newZone);

        SetZone(newZone);
    }

    /// <summary>
    /// Look up which zone owns a specific world position.
    /// </summary>
    public ZoneDefinition GetZoneAtPosition(Vector3 worldPos)
    {
        int cx = Mathf.FloorToInt(worldPos.x * _invChunkSize);
        int cy = Mathf.FloorToInt(worldPos.y * _invChunkSize);
        ZoneDefinition zone;
        _chunkCoordToZone.TryGetValue(new Vector2Int(cx, cy), out zone);
        return zone;
    }

    #endregion

    #region SetZone

    public void SetZone(ZoneDefinition newZone) 
    {
        if (newZone == CurrentZone || newZone == null) return;
        
        PreviousZone = CurrentZone;
        CurrentZone = newZone;
        UpdateLocalization();
        WorldEvents.RaiseZoneChanged(PreviousZone, newZone, ZoneName);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        string prevId = PreviousZone != null ? PreviousZone.zoneId : "null";
        string newId = newZone != null ? newZone.zoneId : "null";
        LogManager.Trace(string.Concat("[ZoneTracker] Zone transition: ", prevId, " → ", newId));
#endif
    }

    #endregion

    #region Events
    /*
    public void Subscribe()
    {
        WorldEvents.OnZoneChanged += HandleZoneChanged;
    }

    public void Unsubscribe()
    {
        WorldEvents.OnZoneChanged -= HandleZoneChanged;
    }

    private void HandleZoneChanged(ZoneDefinition previousZone, ZoneDefinition newZone, string newName) 
    {
        if (previousZone == newZone) return;
    }
    */
    #endregion

    #region Localization
    private void UpdateLocalization() 
    {
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Zone,
            CurrentZone.zoneId,
            new [] { LocalizationField.Name }
        );
    }
    #endregion

}
