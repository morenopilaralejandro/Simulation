using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemImportant : Item
{
    #region Components

    private ItemComponentImportant importantComponent;

    #endregion

    #region Initialize

    public ItemImportant(ItemDataImportant data) : base(data)
    {
        InitializeItemImportant(data);
    }

    private void InitializeItemImportant(ItemDataImportant data)
    {
        importantComponent = new ItemComponentImportant(data, this);
    }

    #endregion

    #region API

    // importantComponent
    public bool PlaceHolder => importantComponent.PlaceHolder;

    #endregion
}
