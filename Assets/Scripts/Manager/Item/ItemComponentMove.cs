using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Item;
using Simulation.Enums.Move;

public class ItemComponentMove
{
    private Item item;
    public string MoveId { get; private set; }

    public ItemComponentMove(ItemDataMove itemDataMove, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        MoveId = itemDataMove.MoveId;
    }
}
