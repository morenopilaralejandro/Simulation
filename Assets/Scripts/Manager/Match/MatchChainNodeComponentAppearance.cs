using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentAppearance
{
    public string IconAddress { get; private set; }

    public MatchChainNodeComponentAppearance(MatchChainNodeData data) { }

    public void SetIconAddress(string address) => IconAddress = address;
}
