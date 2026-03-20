using UnityEngine;
using Simulation.Enums.Item;

public class ItemMove : Item
{
    #region Components

    private ItemComponentMove moveComponent;

    #endregion

    #region Initialize

    public ItemMove(ItemDataMove data) : base(data)
    {
        InitializeItemMove(data);
    }

    private void InitializeItemMove(ItemDataMove data)
    {
        moveComponent = new ItemComponentMove(data, this);
    }

    #endregion

    #region API

    // moveComponent
    public string MoveId => moveComponent.MoveId;

    #endregion
}
