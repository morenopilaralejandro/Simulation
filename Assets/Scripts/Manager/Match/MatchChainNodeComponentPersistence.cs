using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class MatchChainNodeComponentPersistence
{
    #region Fields

    private MatchChainNode matchChainNode;
    public bool IsNodeUnlocked { get; private set; }

    #endregion        

    #region Construcor

    public MatchChainNodeComponentPersistence(MatchChainNodeData data, MatchChainNode obj, MatchChainNodeSaveData saveData)
    {
        this.matchChainNode = obj;
        IsNodeUnlocked = false;
        if (saveData == null) return;
        IsNodeUnlocked = saveData.IsNodeUnlocked;
    }

    #endregion

    #region Import

    public void Import(MatchChainNodeSaveData saveData)
    {
        matchChainNode = MatchChainNodeFactory.CreateByIdAndCategory(saveData.MatchChainNodeId, saveData.NodeCategory, saveData);
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
            /*
            case MatchChainNodeLock matchChainNodeLock:
                saveData.IsLockOpen = matchChainNodeLock.IsLockOpen;;
                break;
            */
        }

        return saveData;
    }

    #endregion

    #region Logic

    public void SetIsNodeUnlocked(bool boolValue) => IsNodeUnlocked = boolValue;

    #endregion
}
