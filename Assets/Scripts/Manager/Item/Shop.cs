using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Localization;

public class Shop
{
    #region Components

    private ShopComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private ShopComponentItems itemsComponent;

    #endregion

    #region Initialize

    public Shop(ShopData data) 
    {
        Initialize(data);
    }

    public void Initialize(ShopData data)
    {
        attributesComponent = new ShopComponentAttributes(data);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Shop,
            data.ShopId,
            new[] { LocalizationField.Name }
        );
        itemsComponent = new ShopComponentItems(data);
    }

    #endregion

    #region API

    // attributesComponent
    public string ShopId => attributesComponent.ShopId;
    public CurrencyType CurrencyType => attributesComponent.CurrencyType;

    // localizationComponent
    public string ShopName => localizationStringComponent.GetString(LocalizationField.Name);

    // itemsComponent
    public IReadOnlyList<Item> Items => itemsComponent.Items;

    #endregion

}
