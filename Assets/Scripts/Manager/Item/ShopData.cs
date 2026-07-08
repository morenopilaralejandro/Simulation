using UnityEngine;
using Aremoreno.Enums.Item;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopData", menuName = "ScriptableObject/Item/ShopData")]
public class ShopData : ScriptableObject
{
    public string ShopId;
    public CurrencyType CurrencyType;
    public List<string> ItemIds;
}
