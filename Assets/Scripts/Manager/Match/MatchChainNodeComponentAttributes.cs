using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentAttributes
{
    public string MatchChainNodeId { get; private set; }
    public MatchChainNodeCategory NodeCategory { get; private set; }
    public int NodeIndex { get; private set; }

    public MatchChainNodeComponentAttributes(MatchChainNodeData data)
    {
        MatchChainNodeId = data.MatchChainNodeId;
        NodeCategory = data.NodeCategory;
        NodeIndex = data.NodeIndex;
    }
}
