using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ShopComponentItems
{
    private List<Item> items = new ();
    public IReadOnlyList<Item> Items => items;

    public ShopComponentItems(ShopData data)
    {
        Initialize(data);
    }

    public void Initialize(ShopData data)
    {
        foreach (string id in data.ItemIds) 
            items.Add(ItemFactory.CreateById(id));
    }
}
