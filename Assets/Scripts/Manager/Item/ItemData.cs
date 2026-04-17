using UnityEngine;
using Aremoreno.Enums.Item;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemId;
    public ItemCategory Category;
    public ItemSpriteType SpriteType;
    public ItemSpriteColor SpriteColor;
    public ItemUsageContext UsageContext;

    public bool IsSellable;
    public int PriceBuyGold;
    public int PriceSellGold;

    public bool IsDiscardable;
    public bool IsStackable;
    public int MaxStack;
}
