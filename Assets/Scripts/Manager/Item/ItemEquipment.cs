using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;

public class ItemEquipment : Item
{
    #region Components

    private ItemComponentEquipment equipmentComponent;

    #endregion

    #region Initialize

    public ItemEquipment(ItemDataEquipment data) : base(data)
    {
        InitializeItemEquipment(data);
    }

    private void InitializeItemEquipment(ItemDataEquipment data)
    {
        equipmentComponent = new ItemComponentEquipment(data, this);
    }

    #endregion

    #region API

    // equipmentComponent
    public EquipmentType EquipmentType => equipmentComponent.EquipmentType;
    public IReadOnlyDictionary<Stat, int> EquipmentStats => equipmentComponent.EquipmentStats;

    #endregion
}
