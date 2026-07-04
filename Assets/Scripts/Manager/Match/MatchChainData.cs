using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

[CreateAssetMenu(fileName = "MatchChainData", menuName = "ScriptableObject/Match/MatchChainData")]
public class MatchChainData : ScriptableObject
{
    public string MatchChainId;
    public List<MatchChainNodeData> NodeDataList;
}
