using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemCharacter : Item
{
    #region Components

    private ItemComponentCharacter characterComponent;

    #endregion

    #region Initialize

    public ItemCharacter(ItemDataCharacter data) : base(data)
    {
        InitializeItemCharacter(data);
    }

    private void InitializeItemCharacter(ItemDataCharacter data)
    {
        characterComponent = new ItemComponentCharacter(data, this);
    }

    #endregion

    #region API

    // characterComponent
    public string CharacterId => characterComponent.CharacterId;

    #endregion
}
