using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Item;

public class ItemComponentFormation
{
    private Item item;
    public string FormationId { get; private set; }

    public ItemComponentFormation(ItemDataFormation itemDataFormation, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        FormationId = itemDataFormation.FormationId;
    }
}
