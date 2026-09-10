using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class SelectorEquipmentListItem : SelectorListItem<ItemEquipment>
{
    [Header("UI")]
    [SerializeField] private ItemUI itemUI;

    protected override void OnBind(ItemEquipment obj)
    {
        this.Selected += HandleItemSelected;
        itemUI.SetData(obj, ItemManager.Instance.GetItemCount(obj));
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;
        itemUI.Clear();
    }

    public void HandleItemSelected(SelectorListItem<ItemEquipment> listItem)
    {
        UIEvents.RaiseEquipmentStatLayoutUpdateRequested(listItem.Data);
    }
}
