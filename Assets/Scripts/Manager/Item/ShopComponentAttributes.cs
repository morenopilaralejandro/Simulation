using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ShopComponentAttributes
{
    public string ShopId { get; private set; }
    public CurrencyType CurrencyType { get; private set; }

    public ShopComponentAttributes(ShopData data)
    {
        Initialize(data);
    }

    public void Initialize(ShopData data)
    {
        ShopId = data.ShopId;
        CurrencyType = data.CurrencyType;
    }
}
