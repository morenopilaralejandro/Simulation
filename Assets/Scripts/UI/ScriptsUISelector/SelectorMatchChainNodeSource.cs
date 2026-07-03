using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Item;

public class SelectorMatchChainNodeSource : ISelectorSource<MatchChainNode>
{
    private MatchChain matchChain;
    public MatchChain MatchChain => matchChain;

    public SelectorMatchChainNodeSource() {}
    public SelectorMatchChainNodeSource(string id) 
    {
        this.matchChain = StorySystemManager.Instance.GetMatchChain(id);
    }

    public IEnumerable<MatchChainNode> Enumerate()
        => matchChain.Nodes;
}
