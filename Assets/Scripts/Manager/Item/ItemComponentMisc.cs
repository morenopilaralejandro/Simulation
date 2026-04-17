using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentMisc
{
    private Item item;
    public bool PlaceHolder { get; private set; }

    public ItemComponentMisc(ItemDataMisc itemDataMisc, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        PlaceHolder = itemDataMisc.PlaceHolder;
    }
}
