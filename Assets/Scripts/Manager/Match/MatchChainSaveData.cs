using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

[System.Serializable]
public class MatchChainSaveData
{
    //attributes
    public string MatchChainId;

    public List<MatchChainNodeSaveData> Nodes;
}
