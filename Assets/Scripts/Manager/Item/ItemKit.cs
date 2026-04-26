using UnityEngine;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Kit;

public class ItemKit : Item
{
    #region Components

    private ItemComponentKit kitComponent;

    #endregion

    #region Initialize

    public ItemKit(ItemDataKit data) : base(data)
    {
        InitializeItemKit(data);
    }

    private void InitializeItemKit(ItemDataKit data)
    {
        kitComponent = new ItemComponentKit(data, this);
    }

    #endregion

    #region API

    // kitComponent
    public string KitId => kitComponent.KitId;

    #endregion
}
