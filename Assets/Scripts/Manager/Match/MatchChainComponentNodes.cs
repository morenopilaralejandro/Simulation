using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Match;

public class MatchChainComponentNodes
{
    private MatchChain matchChain;
    //public string MatchChainId { get; private set; }
    public List<MatchChainNode> Nodes = new();

    public MatchChainComponentNodes(MatchChainData data, MatchChain obj, MatchChainSaveData saveData)
    {
        matchChain = obj;

        if (saveData == null) 
        {
            foreach (var nodeData in data.NodeDataList) 
                Nodes.Add(MatchChainNodeFactory.CreateByIdAndCategory(nodeData.MatchChainNodeId, nodeData.NodeCategory));
        } else 
        {
            foreach (var nodeSaveData in saveData.Nodes)
                Nodes.Add(MatchChainNodeFactory.CreateByIdAndCategory(nodeSaveData.MatchChainNodeId, nodeSaveData.NodeCategory, nodeSaveData));
        }

        SortNodexByIndex();
        Nodes[0]?.SetIsNodeUnlocked(true);
    }

    public void SortNodexByIndex() 
    {
        Nodes.Sort((a, b) => a.NodeIndex.CompareTo(b.NodeIndex));
    }
}
