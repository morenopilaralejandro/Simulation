using UnityEngine;
using Simulation.Enums.Item;

public class ItemMisc : Item
{
    #region Components

    private ItemComponentMisc miscComponent;

    #endregion

    #region Initialize

    public ItemMisc(ItemDataMisc data) : base(data)
    {
        InitializeItemMisc(data);
    }

    private void InitializeItemMisc(ItemDataMisc data)
    {
        miscComponent = new ItemComponentMisc(data, this);
    }

    #endregion

    #region API

    // miscComponent
    public bool PlaceHolder => miscComponent.PlaceHolder;

    #endregion
}
