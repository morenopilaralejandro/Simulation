using UnityEngine;

public class SelectorItemStorageSlotBagAction : ISelectorClickAction<ItemStorageSlot>
{
    public void Execute(ItemStorageSlot itemStorageSlot, IClosableMenu menu)
    {
        //AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        //UIEvents.RaiseSelectorItemStorageSlotSideActionClicked(itemStorageSlot);
    }
}
