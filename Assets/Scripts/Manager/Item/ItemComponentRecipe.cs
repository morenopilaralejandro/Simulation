using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentRecipe
{
    private Item item;
    public string RecipeId { get; private set; }

    public ItemComponentRecipe(ItemDataRecipe itemDataRecipe, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        RecipeId = itemDataRecipe.RecipeId;
    }
}
