using UnityEngine;
using System;
using System.Collections.Generic;

public static class MatchChainFactory
{
    //public static MatchChain Create() { }

    public static MatchChain Create(MatchChainData data, MatchChainSaveData saveData = null)
    {
        return new MatchChain(data, saveData);
    }

    public static MatchChain Create(MatchChainSaveData saveData)
    {
        return CreateById(saveData.MatchChainId, saveData);
    }

    public static MatchChain CreateById(string id, MatchChainSaveData saveData = null)
    {
        return Create(DatabaseManager.Instance.GetMatchChainData(id), saveData);
    }
}
