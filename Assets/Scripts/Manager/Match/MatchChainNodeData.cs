using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

[CreateAssetMenu(fileName = "MatchChainNodeData", menuName = "ScriptableObject/Match/MatchChainNodeData")]
public class MatchChainNodeData : ScriptableObject
{
    public string MatchChainNodeId;
    public string MatchChainId;
    public int NodeIndex;
    public MatchChainNodeCategory NodeCategory;
}
