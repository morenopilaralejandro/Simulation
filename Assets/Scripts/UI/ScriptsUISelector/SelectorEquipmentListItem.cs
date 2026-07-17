using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class SelectorEquipmentListItem : SelectorListItem<ItemEquipment>
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private Image imageIcon;
    private readonly AddressableBinding<Sprite> _bindingIcon = new();

    protected override void OnBind(ItemEquipment obj)
    {
        this.Selected += HandleItemSelected;

        textName.text = obj.ItemName;
        imageIcon.color = obj.IconColor;
        _ = SetIconAsync(obj.IconSpriteAddress);
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;

        textName.text = "";
        _bindingIcon.Release();
        _bindingIcon.Cancel();
        imageIcon.sprite = null;
    }

    public void HandleItemSelected(SelectorListItem<ItemEquipment> listItem)
    {
        UIEvents.RaiseEquipmentStatLayoutUpdateRequested(listItem.Data);
    }

    public async Task SetIconAsync(string address)
    {
        imageIcon.enabled = false;
        var asset = await _bindingIcon.LoadAsync(address);
        imageIcon.sprite = asset;
        imageIcon.enabled = true;
    }

}
