using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectorItemStorageSlotSideListItem : SelectorListItem<ItemStorageSlot>
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;

    protected override void OnBind(ItemStorageSlot obj)
    {
        this.Selected += HandleItemSelected;

        textName.text = obj.Item.ItemName;
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;

        textName.text = "";
    }

    public void HandleItemSelected(SelectorListItem<ItemStorageSlot> listItem)
    {
        UIEvents.RaiseSelectorItemStorageSlotSideListItemSelected(listItem.Data);
    }
}
