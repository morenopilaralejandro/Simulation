using UnityEngine;
using Aremoreno.Enums.Item;

public class SelectorItemStorageSlotBagActionUse : ISelectorClickAction<ItemStorageSlot>
{
    public void Execute(ItemStorageSlot itemStorageSlot, IClosableMenu menu)
    {
        if(itemStorageSlot.Item.Category == ItemCategory.Recovery || itemStorageSlot.Item.Category == ItemCategory.Move)
            UIEvents.RaiseMenuItemUseOpenRequested(itemStorageSlot.Item);
    }
}
