using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentImportant
{
    private Item item;
    public bool PlaceHolder { get; private set; }

    public ItemComponentImportant(ItemDataImportant itemDataImportant, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        PlaceHolder = itemDataImportant.PlaceHolder;
    }
}
