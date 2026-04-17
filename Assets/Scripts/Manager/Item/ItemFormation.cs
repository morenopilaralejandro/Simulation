using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemFormation : Item
{
    #region Components

    private ItemComponentFormation formationComponent;

    #endregion

    #region Initialize

    public ItemFormation(ItemDataFormation data) : base(data)
    {
        InitializeItemFormation(data);
    }

    private void InitializeItemFormation(ItemDataFormation data)
    {
        formationComponent = new ItemComponentFormation(data, this);
    }

    #endregion

    #region API

    // formationComponent
    public string FormationId => formationComponent.FormationId;

    #endregion
}
