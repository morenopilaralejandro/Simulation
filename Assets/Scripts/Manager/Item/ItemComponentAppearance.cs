using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentAppearance
{
    private Item item;

    public ItemSpriteType SpriteType { get; private set; }
    public ItemSpriteColor SpriteColor { get; private set; }
    public Sprite IconSprite { get; private set; }
    public Color IconColor { get; private set; }

    public ItemComponentAppearance(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        Initialize(itemData, item, itemSaveData);
    }

    public async void Initialize(ItemData itemData, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;

        SpriteType = itemData.SpriteType;
        SpriteColor = itemData.SpriteColor;
        IconColor = ColorManager.GetItemSpriteColor(SpriteColor);
        await LoadAsync();
    }

    #region Async Loading

    private async Task LoadAsync()
    {
        IconSprite = await SpriteAtlasManager.Instance.GetItemIcon(SpriteType.ToString().ToLower());
    }

    #endregion
}
