using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentText
{
    private MatchChainNodeText matchChainNodeText;

    public MatchChainNodeComponentText(MatchChainNodeDataText data, MatchChainNodeText obj, MatchChainNodeSaveData saveData = null)
    {
        this.matchChainNodeText = obj;
    }
}
