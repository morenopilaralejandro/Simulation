using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Aremoreno.Enums.UI;

public class SelectorItemStorageSlotBagListItem : SelectorListItem<ItemStorageSlot>
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textAmount;
    [SerializeField] private Image imageIcon;
    private readonly AddressableBinding<Sprite> _bindingIcon = new();
    private MenuBagMode mode;

    public void SetMode(MenuBagMode mode) => this.mode = mode;

    protected override void OnBind(ItemStorageSlot obj)
    {
        this.Selected += HandleItemSelected;

        textName.text = obj.Item.ItemName;
        textAmount.text = obj.Count.ToString();
        imageIcon.color = obj.Item.IconColor;
        _ = SetIconAsync(obj.Item.IconSpriteAddress);
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;

        textName.text = "";
        textAmount.text = "";
        _bindingIcon.Release();
        _bindingIcon.Cancel();
        imageIcon.sprite = null;
    }

    public void HandleItemSelected(SelectorListItem<ItemStorageSlot> listItem)
    {
        UIEvents.RaiseBagDescriptionUpdated(listItem.Data.Item);
    }

    public async Task SetIconAsync(string address)
    {
        imageIcon.enabled = false;
        var asset = await _bindingIcon.LoadAsync(address);
        imageIcon.sprite = asset;
        imageIcon.enabled = true;
    }

}
