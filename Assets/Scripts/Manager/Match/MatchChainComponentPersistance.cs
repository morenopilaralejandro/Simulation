using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class MatchChainComponentPersistance
{
    #region Fields

    private MatchChain matchChain;

    #endregion        

    #region Construcor

    public MatchChainComponentPersistance(MatchChainData data, MatchChain obj, MatchChainSaveData saveData)
    {
        this.matchChain = obj;
    }

    #endregion

    #region Import

    public void Import(MatchChainSaveData saveData)
    {
        matchChain.Initialize(
            DatabaseManager.Instance.GetMatchChainData(saveData.MatchChainId), 
            saveData);
    }

    #endregion

    #region Export

    public MatchChainSaveData Export()
    {
        return new MatchChainSaveData
        {
            MatchChainId = matchChain.MatchChainId,
            Nodes = GetNodeSaveData()
        };
    }

    #endregion

    #region Helpers

    private List<MatchChainNodeSaveData> GetNodeSaveData() 
    {
        List<MatchChainNodeSaveData> list = new ();

        foreach(MatchChainNode node in matchChain.Nodes) 
            list.Add(node.Export());

        return list;
    }

    #endregion
}
