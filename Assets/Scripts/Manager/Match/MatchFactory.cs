using UnityEngine;
using System;
using System.Collections.Generic;

public static class MatchFactory
{
    //public static Match Create() { }

    public static Match Create(MatchData data)
    {
        return new Match(data);
    }

    /*
    public static MatchChain Create(MatchChainSaveData saveData)
    {
        return new MatchChain(null, saveData);
    }
    */

    public static Match CreateById(string id)
    {
        return Create(DatabaseManager.Instance.GetMatchData(id));
    }
}
