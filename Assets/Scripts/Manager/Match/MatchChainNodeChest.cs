using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeChest : MatchChainNode
{
    #region Components

    private MatchChainNodeComponentChest chestComponent;

    #endregion

    #region Initialize

    public MatchChainNodeChest(MatchChainNodeDataChest data, MatchChainNodeSaveData savedata = null) : base(data, savedata)
    {
        chestComponent = new MatchChainNodeComponentChest(data, this);
    }

    #endregion

    #region API
    //chestComponent
    public string ItemId => chestComponent.ItemId;
    public bool IsChestOpen => chestComponent.IsChestOpen;
    public void Open() => chestComponent.Open();

    #endregion
}
