using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNode
{
    #region Components

    private MatchChainNodeComponentAttributes attributesComponent;
    private MatchChainNodeComponentAppearance appearanceComponent;
    private MatchChainNodeComponentPersistance persistanceComponent;

    #endregion

    #region Constructor

    public MatchChainNode(MatchChainNodeData data, MatchChainNodeSaveData saveData = null) 
    {
        attributesComponent = new MatchChainNodeComponentAttributes(data);
        appearanceComponent = new MatchChainNodeComponentAppearance(data);
        persistanceComponent = new MatchChainNodeComponentPersistance(data, this, saveData);
    }

    #endregion

    #region API

    // attributesComponent
    public string MatchChainNodeId => attributesComponent.MatchChainNodeId;
    public MatchChainNodeCategory NodeCategory => attributesComponent.NodeCategory;
    public int NodeIndex => attributesComponent.NodeIndex;

    // appearanceComponent
    public string IconAddress => appearanceComponent.IconAddress;
    public void SetIconAddress(string address) => appearanceComponent.SetIconAddress(address);

    #endregion

}
