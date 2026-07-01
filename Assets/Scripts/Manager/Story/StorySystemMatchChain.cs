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
    private MatchChain auxMatchChain;

    private Dictionary<string, MatchChain> dict = new Dictionary<string, MatchChain>();

    public StorySystemMatchChain()
    {
        InitializeFromDatabase();
    }

    private void InitializeFromDatabase()
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

    public void TryUnlockNextNode(string sourceNodeId) 
    {
        auxNodeData = DatabaseManager.Instance.GetMatchChainNodeData(sourceNodeId);
        auxMatchChain = dict[auxNodeData.MatchChainId];
        auxMatchChain.GetNodeByIndex(auxNodeData.NodeIndex++)?.SetIsNodeUnlocked(true);
    }

}
