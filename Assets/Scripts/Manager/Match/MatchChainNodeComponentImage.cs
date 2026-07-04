using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentImage
{
    private MatchChainNodeImage matchChainNodeImage;
    public string ImageAddress { get; private set; }

    public MatchChainNodeComponentImage(MatchChainNodeDataImage data, MatchChainNodeImage obj, MatchChainNodeSaveData saveData = null)
    {
        this.matchChainNodeImage = obj;
        ImageAddress = data.ImageAddress;
    }
}
