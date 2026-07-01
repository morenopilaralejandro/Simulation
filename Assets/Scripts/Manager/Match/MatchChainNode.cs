using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNode
{
    #region Components

    private MatchChainNodeComponentAttributes attributesComponent;
    private MatchChainNodeComponentAppearance appearanceComponent;
    private MatchChainNodeComponentPersistence persistenceComponent;

    #endregion

    #region Constructor

    public MatchChainNode(MatchChainNodeData data, MatchChainNodeSaveData saveData = null) 
    {
        attributesComponent = new MatchChainNodeComponentAttributes(data);
        appearanceComponent = new MatchChainNodeComponentAppearance(data);
        persistenceComponent = new MatchChainNodeComponentPersistence(data, this, saveData);
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

    //persistenceComponent
    public bool IsNodeUnlocked => persistenceComponent.IsNodeUnlocked;
    public void Import(MatchChainNodeSaveData saveData) => persistenceComponent.Import(saveData);
    public MatchChainNodeSaveData Export() => persistenceComponent.Export();
    public void SetIsNodeUnlocked(bool boolValue) => persistenceComponent.SetIsNodeUnlocked(boolValue);

    #endregion

}
