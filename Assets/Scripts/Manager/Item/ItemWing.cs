using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemWing : Item
{
    #region Components

    private ItemComponentWing wingComponent;

    #endregion

    #region Initialize

    public ItemWing(ItemDataWing data) : base(data)
    {
        InitializeItemWing(data);
    }

    private void InitializeItemWing(ItemDataWing data)
    {
        wingComponent = new ItemComponentWing(data, this);
    }

    #endregion

    #region API

    // WingComponent
    public string WingId => wingComponent.WingId;

    #endregion
}
