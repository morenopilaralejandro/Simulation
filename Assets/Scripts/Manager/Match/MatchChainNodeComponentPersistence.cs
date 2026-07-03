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
    public bool IsNodeCompleted { get; private set; }

    #endregion        

    #region Construcor

    public MatchChainNodeComponentPersistence(MatchChainNodeData data, MatchChainNode obj, MatchChainNodeSaveData saveData)
    {
        this.matchChainNode = obj;
        IsNodeUnlocked = false;
        IsNodeCompleted = false;
        if (saveData == null) return;
        IsNodeUnlocked = saveData.IsNodeUnlocked;
        IsNodeCompleted = saveData.IsNodeCompleted;
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
        saveData.IsNodeCompleted = matchChainNode.IsNodeCompleted;

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
    public void SetIsNodeCompleted(bool boolValue) => IsNodeCompleted = boolValue;

    public void Complete()
    {
        LogManager.Trace($"COMPLETE START | id={matchChainNode.MatchChainNodeId} | hash={matchChainNode.GetHashCode()} | completed={IsNodeCompleted}");

        if (IsNodeCompleted)
        {
            LogManager.Trace("Already completed");
            return;
        }

        IsNodeCompleted = true;

        LogManager.Trace($"COMPLETE AFTER | id={matchChainNode.MatchChainNodeId} | completed={IsNodeCompleted}");

        StorySystemManager.Instance.TryUnlockNextNode(matchChainNode.MatchChainNodeId);

        UIEvents.RaiseMatchChainNodeUpdated(matchChainNode);
    }

    public void Unlock()
    {
        LogManager.Trace($"UNLOCK START | id={matchChainNode.MatchChainNodeId} | hash={matchChainNode.GetHashCode()} | unlocked={IsNodeUnlocked}");

        if (IsNodeUnlocked)
        {
            LogManager.Trace("Already unlocked");
            return;
        }

        IsNodeUnlocked = true;

        LogManager.Trace($"UNLOCK AFTER | unlocked={IsNodeUnlocked}");

        UIEvents.RaiseMatchChainNodeUpdated(matchChainNode);
    }

    #endregion
}
