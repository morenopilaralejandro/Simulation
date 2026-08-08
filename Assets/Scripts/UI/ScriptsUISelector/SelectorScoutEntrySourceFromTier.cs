using UnityEngine;
using System.Collections.Generic;

public class SelectorScoutEntrySourceFromTier : ISelectorSource<ScoutEntry>
{
    private ScoutTier scoutTier;
    public ScoutTier ScoutTier => scoutTier;

    public SelectorScoutEntrySourceFromTier(ScoutTier scoutTier)
    {
        this.scoutTier = scoutTier;
    }
    
    public void SetScoutTier(ScoutTier scoutTier)
    {
        this.scoutTier = scoutTier;
    }

    public IEnumerable<ScoutEntry> Enumerate() => scoutTier.GetEntries();
}
