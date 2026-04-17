using UnityEngine;
using Aremoreno.Enums.Item;

public class ItemRecovery : Item
{
    #region Components

    private ItemComponentRecovery recoveryComponent;

    #endregion

    #region Initialize

    public ItemRecovery(ItemDataRecovery data) : base(data)
    {
        InitializeItemRecovery(data);
    }

    private void InitializeItemRecovery(ItemDataRecovery data)
    {
        recoveryComponent = new ItemComponentRecovery(data, this);
    }

    #endregion

    #region API

    // recoveryComponent
    public int RecoveryAmountHp => recoveryComponent.RecoveryAmountHp;
    public int RecoveryAmountSp => recoveryComponent.RecoveryAmountSp;

    #endregion
}
