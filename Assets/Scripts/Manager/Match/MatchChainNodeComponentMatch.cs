using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentMatch
{
    private MatchChainNodeMatch matchChainNodeMatch;
    public string MatchId { get; private set; }
    public MatchRank MatchRank { get; private set; }

    public MatchChainNodeComponentMatch(MatchChainNodeDataMatch data, MatchChainNodeMatch obj, MatchChainNodeSaveData saveData = null)
    {
        this.matchChainNodeMatch = obj;
        MatchId = data.MatchId;
    }

    public void SetMatchRank(MatchRank rank) => MatchRank = rank;
    public void SetMatchRankBest(MatchRank rank) => MatchRank = (MatchRank)Mathf.Max((int)MatchRank, (int)rank);
}
