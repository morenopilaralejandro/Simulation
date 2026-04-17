using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemComponentRecovery
{
    private Item item;
    public int RecoveryAmountHp { get; private set; }
    public int RecoveryAmountSp { get; private set; }

    public ItemComponentRecovery(ItemDataRecovery itemDataRecovery, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        RecoveryAmountHp = itemDataRecovery.RecoveryAmountHp;
        RecoveryAmountSp = itemDataRecovery.RecoveryAmountSp;
    }
}
