using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.Color;
using Aremoreno.Enums.Item;

public class ItemComponentAppearance
{
    private Item item;

    public ItemSpriteType SpriteType { get; private set; }
    public ColorGeneric SpriteColor { get; private set; }
    public string IconSpriteAddress { get; private set; }
    public Color IconColor { get; private set; }

    public ItemComponentAppearance(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        Initialize(itemData, item, itemSaveData);
    }

    public void Initialize(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;

        SpriteType = itemData.SpriteType;
        SpriteColor = itemData.SpriteColor;
        IconColor = ColorManager.GetGenericColor(SpriteColor);
        IconSpriteAddress = AddressableLoader.GetItemIconAddress(itemData.SpriteType.ToString().ToLower());
    }
}
