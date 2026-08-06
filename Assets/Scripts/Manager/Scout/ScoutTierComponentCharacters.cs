using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Scout;

public class ScoutTierComponentCharacters
{
    public int CharacterCost { get; private set; }
    public int CharacterLevel { get; private set; }
    public List<string> CharacterIds { get; private set; }

    public ScoutTierComponentCharacters(ScoutTierData data)
    {
        CharacterCost = data.CharacterCost;
        CharacterLevel = data.CharacterLevel;
        CharacterIds = data.CharacterIds != null
            ? new List<string>(data.CharacterIds)
            : new List<string>();
    }
}
