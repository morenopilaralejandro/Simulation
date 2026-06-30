using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class MatchChainNodeComponentPersistance
{
    #region Fields

    private MatchChainNode matchChainNode;
    public bool IsNodeUnlocked { get; private set; }

    #endregion        

    #region Construcor

    public MatchChainNodeComponentPersistance(MatchChainNodeData data, MatchChainNode obj, MatchChainNodeSaveData saveData)
    {
        this.matchChainNode = obj;
        IsNodeUnlocked = saveData.IsNodeUnlocked;
    }

    #endregion

    #region Import

    public void Import(MatchChainNodeSaveData saveData)
    {
        matchChainNode.Initialize(
            DatabaseManager.Instance.GetMatchChainNodeData(saveData.MatchChainNodeId), 
            saveData);
    }

    #endregion

    #region Export

    public MatchChainNodeSaveData Export()
    {   
        MatchChainNodeSaveData saveData = new MatchChainNodeSaveData();

        saveData.MatchChainNodeId = matchChainNode.MatchChainNodeId;
        saveData.NodeCategory = matchChainNode.NodeCategory;
        saveData.IsNodeUnlocked = matchChainNode.IsNodeUnlocked;

        switch (matchChainNode)
        {
            case MatchChainNodeMatch matchChainNodeMatch:
                saveData.MatchRank = matchChainNodeMatch.MatchRank;
                break;
        
            case MatchChainNodeChest matchChainNodeChest:
                saveData.IsChestOpen = matchChainNodeChest.IsChestOpen;
                break;

            case MatchChainNodeLock matchChainNodeLock:
                saveData.IsLockOpen = matchChainNodeLock.IsLockOpen;;
                break;
        }

    }

    #endregion

    #region Logic

    public void SetIsNodeUnlocked(bool boolValue) => IsNodeUnlocked = boolValue;

    #endregion
}
