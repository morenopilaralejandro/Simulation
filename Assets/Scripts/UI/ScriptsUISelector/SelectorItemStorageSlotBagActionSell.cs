using UnityEngine;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class SelectorItemStorageSlotBagActionSell : ISelectorClickAction<ItemStorageSlot>
{
    public void Execute(ItemStorageSlot itemStorageSlot, IClosableMenu menu)
    {
        //AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        UIEvents.RaisePickerAmountOpened(
            PickerAmountMode.Sell,
            itemStorageSlot.Item,
            1,
            itemStorageSlot.Count,
            CurrencyType.Gold
        );
    }

    /*

int gold = ItemManager.Instance.GetAmount(CurrencyType.Gold);
int price = item.GetPriceBuy(CurrencyType.Gold);

int maxAffordable = price > 0 ? gold / price : 999;
amountMax = Mathf.Min(999, maxAffordable);

Or, if you also have a stock limit (for example the shop only has 25 available):

amountMax = Mathf.Min(999, stockAvailable, maxAffordable);

    */
}
