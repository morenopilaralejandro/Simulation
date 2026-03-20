using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Item;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

public class Item
{
    #region Components

    private ItemComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private ItemComponentAppearance appearanceComponent;
    private ItemComponentInventory inventoryComponent;
    private ItemComponentShop shopComponent;

    #endregion

    #region Initialize

    public Item(ItemData data) 
    {
        Initialize(data);
    }

    public void Initialize(ItemData data)
    {
        attributesComponent = new ItemComponentAttributes(data, this);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Item,
            data.ItemId,
            new[] { LocalizationField.Name, LocalizationField.Description}
        );
        appearanceComponent = new ItemComponentAppearance(data, this);
        inventoryComponent = new ItemComponentInventory(data, this);
        shopComponent = new ItemComponentShop(data, this);
    }

    #endregion

    #region API

    // attributesComponent
    public string ItemId => attributesComponent.ItemId;
    public ItemCategory Category => attributesComponent.Category;

    // localizationComponent
    public string ItemName => localizationStringComponent.GetString(LocalizationField.Name);
    public string ItemDescription => localizationStringComponent.GetString(LocalizationField.Description);

    // appearanceComponent
    public ItemSpriteType SpriteType => appearanceComponent.SpriteType;
    public ItemSpriteColor SpriteColor => appearanceComponent.SpriteColor;
    public Sprite IconSprite => appearanceComponent.IconSprite;
    public Color IconColor => appearanceComponent.IconColor;

    // inventoryComponent
    public ItemUsageContext UsageContext => inventoryComponent.UsageContext;
    public bool IsDiscardable => inventoryComponent.IsDiscardable;
    public bool IsStackable => inventoryComponent.IsStackable;
    public int MaxStack => inventoryComponent.MaxStack;

    // shopComponent
    public bool IsSellable => shopComponent.IsSellable;
    public int PriceBuy => shopComponent.PriceBuy;
    public int PriceSell => shopComponent.PriceSell;

    #endregion
}
