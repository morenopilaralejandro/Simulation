using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Match;

public class MatchChainComponentAttributes
{
    public string MatchChainId { get; private set; }

    public MatchChainComponentAttributes(MatchChainData data)
    {
        MatchChainId = data.MatchChainId;
    }
}
