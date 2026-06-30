using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

[System.Serializable]
public class MatchChainNodeSaveData
{
    //attributes
    public string MatchChainNodeId;
    public MatchChainNodeCategory NodeCategory;

    //persistance
    public bool IsNodeUnlocked;

    //match
    public MatchRank MatchRank;

    //chest
    public bool IsChestOpen;

    //lock
    public bool IsLockOpen;
}
