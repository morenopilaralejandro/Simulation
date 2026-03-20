using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Item;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

public class ItemUsageHandler
{
/*
    private void Use(Item item)
    {
        switch (item)
        {
            case RecoveryItemData recovery:
                HandleRecovery(recovery);
                break;

            case BookItemData book:
                HandleBook(book);
                break;

            case KeyItemData key:
                HandleKeyItem(key);
                break;

            case EquipmentItemData equipment:
                HandleEquipment(equipment);
                break;

            case MaterialItemData:
            case MiscItemData:
            default:
                LogManager.Error($"[ItemUsageHandler] Can't use item with id {item.ItemId}.");
                break;
        }
    }

    private void HandleRecovery(RecoveryItemData recovery)
    {
        // Open party select → pick a target → heal
        partySelectUI.Open(PlayerParty.Instance, (Pokemon target) =>
        {
            target.RestoreHP(recovery.recoverAmountHp);
            target.RestoreSP(recovery.recoverAmountSp);

            if (recovery.curesStatus)
                target.CureStatus();

            Inventory.Instance.RemoveItem(recovery);
            ShowMessage($"{target.pokemonName} was healed!");
        });
    }
*/
}
