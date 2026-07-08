using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Item;

public class SelectorItemShopSource : ISelectorSource<Item>
{
    private Shop shop;
    public Shop Shop => shop;

    public SelectorItemShopSource() {}
    public SelectorItemShopSource(string id) 
    {
        this.shop = ShopFactory.CreateById(id);
    }

    public IEnumerable<Item> Enumerate()
        => shop.Items;
}
