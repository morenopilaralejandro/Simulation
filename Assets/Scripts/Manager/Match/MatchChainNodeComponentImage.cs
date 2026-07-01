using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentImage
{
    private MatchChainNodeImage matchChainNodeImage;

    public MatchChainNodeComponentImage(MatchChainNodeDataImage data, MatchChainNodeImage obj, MatchChainNodeSaveData saveData = null)
    {
        this.matchChainNodeImage = obj;
    }
}
