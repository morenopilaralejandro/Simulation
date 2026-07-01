using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentChest
{
    private MatchChainNodeChest matchChainNodeChest;
    private const string addressIconOpen = "icon-match_chain-chest_open";
    private const string addressIconClose = "icon-match_chain-chest_close"; 
    public string ItemId { get; private set; }
    public bool IsChestOpen { get; private set; }

    public MatchChainNodeComponentChest(MatchChainNodeDataChest data, MatchChainNodeChest obj, MatchChainNodeSaveData saveData = null)
    {
        this.matchChainNodeChest = obj;
        ItemId = data.ItemId;
        IsChestOpen = false;

        if (saveData == null) return;
        IsChestOpen = saveData.IsChestOpen;
        ItemManager.Instance.AddItem(ItemFactory.CreateById(ItemId));
        UpdateAppearence();
    }

    public void Open() 
    {
        if (IsChestOpen) return;
        IsChestOpen = true;
        UpdateAppearence();
    }

    private void UpdateAppearence() 
    {
        if (IsChestOpen)
            matchChainNodeChest.SetIconAddress(addressIconOpen);
        else
            matchChainNodeChest.SetIconAddress(addressIconClose);
    }
}
