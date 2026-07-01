using UnityEngine;
using Aremoreno.Enums.Match;

[CreateAssetMenu(fileName = "MatchChainNodeDataMatch", menuName = "ScriptableObject/Match/MatchChainNodeDataMatch")]
public class MatchChainNodeDataMatch : MatchChainNodeData 
{
    public string MatchId;
    public string DropIdB;
    public string DropIdA;
    public string DropIdS;
}
