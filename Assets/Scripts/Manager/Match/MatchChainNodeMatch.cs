using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeMatch : MatchChainNode
{
    #region Components

    private MatchChainNodeComponentMatch matchComponent;
    private MatchChainNodeComponentDrops dropsComponent;

    #endregion

    #region Initialize

    public MatchChainNodeMatch(MatchChainNodeDataMatch data, MatchChainNodeSaveData savedata = null) : base(data, savedata)
    {
        matchComponent = new MatchChainNodeComponentMatch(data, this);
        dropsComponent = new MatchChainNodeComponentDrops(data);

        //SetIconAddress(); use a helper in manager to get the team emblem form match (instanciate match data from database)
    }

    #endregion

    #region API

    // matchComponent
    public string MatchId => matchComponent.MatchId;
    public MatchRank MatchRank => matchComponent.MatchRank;
    public void SetMatchRank(MatchRank rank) => matchComponent.SetMatchRank(rank);
    public void SetMatchRankBest(MatchRank rank) => matchComponent.SetMatchRankBest(rank);

    //dropsComponent
    public string DropIdB => dropsComponent.DropIdB;
    public string DropIdA => dropsComponent.DropIdA;
    public string DropIdS => dropsComponent.DropIdS;

    public List<ItemReward> GetRewardsByRank(MatchRank rank) => dropsComponent.GetRewardsByRank(rank);

    #endregion
}
