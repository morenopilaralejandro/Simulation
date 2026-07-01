using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeText : MatchChainNode
{
    #region Components

    private MatchChainNodeComponentText textComponent;

    #endregion

    #region Initialize

    public MatchChainNodeText(MatchChainNodeDataText data, MatchChainNodeSaveData savedata = null) : base(data, savedata)
    {
        textComponent = new MatchChainNodeComponentText(data, this);

        SetIconAddress("icon-match_chain-text");
    }

    #endregion

    #region API

    //textComponent
    public string MatchChainText => textComponent.MatchChainText;

    #endregion
}
