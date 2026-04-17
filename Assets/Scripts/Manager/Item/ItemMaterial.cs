using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemMaterial : Item
{
    #region Components

    private ItemComponentMaterial materialComponent;

    #endregion

    #region Initialize

    public ItemMaterial(ItemDataMaterial data) : base(data)
    {
        InitializeItemMaterial(data);
    }

    private void InitializeItemMaterial(ItemDataMaterial data)
    {
        materialComponent = new ItemComponentMaterial(data, this);
    }

    #endregion

    #region API

    // materialComponent
    public bool PlaceHolder => materialComponent.PlaceHolder;

    #endregion
}
