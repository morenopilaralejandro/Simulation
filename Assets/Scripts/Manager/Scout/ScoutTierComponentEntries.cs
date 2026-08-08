using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Scout;

public class ScoutTierComponentEntries
{
    private ScoutTier scoutTier;

    public ScoutTierComponentEntries(ScoutTierData data, ScoutTier scoutTier)
    {
        this.scoutTier = scoutTier;
    }

    public List<ScoutEntry> GetEntries()
    {
        List<ScoutEntry> entries = new List<ScoutEntry>();

        foreach (string id in scoutTier.CharacterIds)
        {
            ScoutEntryData data = new ScoutEntryData
            {
                CharacterId = id,
                Cost = scoutTier.CharacterCost,
                Level = scoutTier.CharacterLevel
            };

            entries.Add(new ScoutEntry(data));
        }

        return entries;
    }
}
