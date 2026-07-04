using System;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public static class MatchEvents
{
    public static event Action<MatchChainNodeMatch, MatchRank> OnMatchChainNodeMatchCompleted;
    public static void RaiseMatchChainNodeMatchCompleted(MatchChainNodeMatch node, MatchRank matchRank)
    {
        OnMatchChainNodeMatchCompleted?.Invoke(node, matchRank);
    }
}
