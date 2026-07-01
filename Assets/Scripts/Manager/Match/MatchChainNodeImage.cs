using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeImage : MatchChainNode
{
    #region Components

    private MatchChainNodeComponentImage imageComponent;

    #endregion

    #region Initialize

    public MatchChainNodeImage(MatchChainNodeDataImage data, MatchChainNodeSaveData savedata = null) : base(data, savedata)
    {
        imageComponent = new MatchChainNodeComponentImage(data, this);

        SetIconAddress("icon-match_chain-image");
    }

    #endregion

    #region API

    #endregion
}
