using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Match;

public class StorySystemMatchChain
{
    private MatchData auxMatchData;
    private TeamData auxTeamData;
    private MatchChainNodeData auxNodeData;

    private Dictionary<string, MatchChain> dict = new Dictionary<string, MatchChain>();

    public StorySystemMatchChain() { }

    public void InitializeFromDatabase()
    {
        foreach (MatchChainData data in DatabaseManager.Instance.DatabaseRegistry.MatchChainData.Data.Values)
        {
            dict[data.MatchChainId] = MatchChainFactory.Create(data);
        }
    }
   
    public void Import(StorySystemSaveData saveData)
    {
        foreach(var chainSaveData in saveData.MatchChainSystemSaveData.MatchChains) 
        {
            dict[chainSaveData.MatchChainId].Import(chainSaveData);
        }
    }

    public MatchChainSystemSaveData Export() 
    {
        MatchChainSystemSaveData saveData = new MatchChainSystemSaveData();
        List<MatchChainSaveData> list = new ();
        foreach(var chain in dict.Values) 
        {
            list.Add(chain.Export());
        }
        saveData.MatchChains = list;
        return saveData;
    }

    public string GetTeamEmblemAddressByMatchId(string matchId) 
    {
        auxMatchData = DatabaseManager.Instance.GetMatchData(matchId);
        auxTeamData = DatabaseManager.Instance.GetTeamData(auxMatchData.TeamId);
        return AddressableLoader.GetTeamEmblemAddress(auxTeamData.EmblemId);
    }

    public MatchChainNode TryGetNextNode(string sourceNodeId)
    {
        auxNodeData = DatabaseManager.Instance.GetMatchChainNodeData(sourceNodeId);

        if (auxNodeData == null) return null;

        return dict[auxNodeData.MatchChainId].GetNodeByIndex(auxNodeData.NodeIndex + 1);
    }

    public void TryUnlockNextNode(string sourceNodeId)
    {
        TryGetNextNode(sourceNodeId)?.Unlock();
    }

    public MatchChain GetMatchChain(string matchChainId)
    {
        return dict.TryGetValue(matchChainId, out var chain) ? chain : null;
    }

    public void TrySetSelectedIndex(MatchChainNode node) 
    {
        dict[node.MatchChainId].SetSelectedIndex(node.NodeIndex);
    }
}
