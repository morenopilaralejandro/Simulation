using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Item;

public class ItemComponentMaterial
{
    private Item item;
    public bool PlaceHolder { get; private set; }

    public ItemComponentMaterial(ItemDataMaterial itemDataMaterial, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        PlaceHolder = itemDataMaterial.PlaceHolder;
    }
}
