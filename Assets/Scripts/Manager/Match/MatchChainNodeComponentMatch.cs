using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentMatch
{
    private MatchChainNode matchChainNode;
    public string MatchId { get; private set; }

    public MatchChainNodeComponentMatch(MatchChainNodeDataMatch data, MatchChainNode obj, MatchChainNodeSaveData saveData = null)
    {
        this.matchChainNode = obj;
        MatchId = data.MatchId;
    }
}
