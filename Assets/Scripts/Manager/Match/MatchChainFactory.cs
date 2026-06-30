using UnityEngine;
using System;
using System.Collections.Generic;

public static class MatchChainFactory
{
    //public static MatchChain Create() { }

    public static MatchChain Create(MatchChainData data)
    {
        return new MatchChain(data);
    }

    public static MatchChain Create(MatchChainSaveData saveData)
    {
        return new MatchChain(null, saveData);
    }

    public static MatchChain CreateById(string id)
    {
        return Create(DatabaseManager.Instance.GetMatchChainData(id));
    }
}
