using UnityEngine;

public class SelectorEquipmentAction : ISelectorClickAction<ItemEquipment>
{
    public void Execute(ItemEquipment itemEquipment, IClosableMenu menu)
    {
        //AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        UIEvents.RaiseSelectorEquipmentActionClicked(itemEquipment);
    }
}
