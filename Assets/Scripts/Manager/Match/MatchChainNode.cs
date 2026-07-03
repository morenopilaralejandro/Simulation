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
    public string MatchChainId => attributesComponent.MatchChainId;
    public MatchChainNodeCategory NodeCategory => attributesComponent.NodeCategory;
    public int NodeIndex => attributesComponent.NodeIndex;
    public bool IsLastNode => attributesComponent.IsLastNode;
    public void SetIsLastNode(bool boolValue) => attributesComponent.SetIsLastNode(boolValue);

    // appearanceComponent
    public string IconAddress => appearanceComponent.IconAddress;
    public void SetIconAddress(string address) => appearanceComponent.SetIconAddress(address);

    //persistenceComponent
    public bool IsNodeUnlocked => persistenceComponent.IsNodeUnlocked;
    public bool IsNodeCompleted => persistenceComponent.IsNodeCompleted;
    public void Import(MatchChainNodeSaveData saveData) => persistenceComponent.Import(saveData);
    public MatchChainNodeSaveData Export() => persistenceComponent.Export();
    public void SetIsNodeUnlocked(bool boolValue) => persistenceComponent.SetIsNodeUnlocked(boolValue);
    public void SetIsNodeCompleted(bool boolValue) => persistenceComponent.SetIsNodeCompleted(boolValue);
    public void Complete() => persistenceComponent.Complete();
    public void Unlock() => persistenceComponent.Unlock();

    #endregion

}
