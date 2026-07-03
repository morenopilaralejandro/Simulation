using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentAttributes
{
    public string MatchChainNodeId { get; private set; }
    public string MatchChainId { get; private set; }
    public MatchChainNodeCategory NodeCategory { get; private set; }
    public int NodeIndex { get; private set; }
    public bool IsLastNode { get; private set; }

    public MatchChainNodeComponentAttributes(MatchChainNodeData data)
    {
        MatchChainNodeId = data.MatchChainNodeId;
        MatchChainId = data.MatchChainId;
        NodeCategory = data.NodeCategory;
        NodeIndex = data.NodeIndex;
        IsLastNode = false;
    }

    public void SetIsLastNode(bool boolValue) => IsLastNode = boolValue;
}
