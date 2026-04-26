using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

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
