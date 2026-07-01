using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentDrops
{
    public string DropIdB { get; private set; }
    public string DropIdA { get; private set; }
    public string DropIdS { get; private set; }

    public MatchChainNodeComponentDrops(MatchChainNodeDataMatch data) 
    {
        DropIdB = data.DropIdB;
        DropIdA = data.DropIdA;
        DropIdS = data.DropIdS;
    }

    public List<ItemReward> GetRewardsByRank(MatchRank rank)
    {
        List<ItemReward> rewards = new List<ItemReward>();

        if (rank >= MatchRank.B && !string.IsNullOrEmpty(DropIdB))
        {
            rewards.Add(new ItemReward
            {
                ItemId = DropIdB,
                Quantity = 1
            });
        }

        if (rank >= MatchRank.A && !string.IsNullOrEmpty(DropIdA))
        {
            rewards.Add(new ItemReward
            {
                ItemId = DropIdA,
                Quantity = 1
            });
        }

        if (rank >= MatchRank.S && !string.IsNullOrEmpty(DropIdS))
        {
            rewards.Add(new ItemReward
            {
                ItemId = DropIdS,
                Quantity = 1
            });
        }

        return rewards;
    }
}
