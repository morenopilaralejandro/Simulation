using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Scout;

public class StorySystemScout
{
    #region Fields

    #endregion

    #region Constructor

    public StorySystemScout() 
    {

    }

    #endregion
   
    #region Peristance

    #endregion

    #region Logic

    public void BuyCharacter(Character character, int price)
    {
        CharacterManager.Instance.AddCharacter(character);
        ItemManager.Instance.Spend(CurrencyType.Gold, price);
    }

    #endregion

    #region Events

    /*

    public void Subscribe() 
    {
        MatchEvents.OnMatchChainNodeMatchCompleted += HandleMatchChainNodeMatchCompleted;
    }

    public void Unsubscribe() 
    {
        MatchEvents.OnMatchChainNodeMatchCompleted -= HandleMatchChainNodeMatchCompleted;
    }

    private void HandleMatchChainNodeMatchCompleted(MatchChainNodeMatch node, MatchRank matchRank)
    {
        node.SetMatchRankBest(matchRank);
        node.Complete();
    }

    */

    #endregion
}
