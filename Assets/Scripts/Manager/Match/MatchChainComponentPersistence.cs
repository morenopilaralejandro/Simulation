using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class MatchChainComponentPersistence
{
    #region Fields

    private MatchChain matchChain;
    public int SelectedIndex { get; private set; }

    #endregion        

    #region Construcor

    public MatchChainComponentPersistence(MatchChainData data, MatchChain obj, MatchChainSaveData saveData)
    {
        this.matchChain = obj;
        SelectedIndex = 0;
        if (saveData == null) return;
        SelectedIndex = saveData.SelectedIndex;
    }

    #endregion

    #region Import

    public void Import(MatchChainSaveData saveData)
    {
        matchChain = MatchChainFactory.Create(saveData);    
    }

    #endregion

    #region Export

    public MatchChainSaveData Export()
    {
        return new MatchChainSaveData
        {
            MatchChainId = matchChain.MatchChainId,
            SelectedIndex = matchChain.SelectedIndex,
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

    public void SetSelectedIndex(int intValue) => SelectedIndex = intValue;

    #endregion
}
