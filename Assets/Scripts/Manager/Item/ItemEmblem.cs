using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemEmblem : Item
{
    #region Components

    private ItemComponentEmblem emblemComponent;

    #endregion

    #region Initialize

    public ItemEmblem(ItemDataEmblem data) : base(data)
    {
        InitializeItemEmblem(data);
    }

    private void InitializeItemEmblem(ItemDataEmblem data)
    {
        emblemComponent = new ItemComponentEmblem(data, this);
    }

    #endregion

    #region API

    // emblemComponent
    public string EmblemId => emblemComponent.EmblemId;

    #endregion
}
