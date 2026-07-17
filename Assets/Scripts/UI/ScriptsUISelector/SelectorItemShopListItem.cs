using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class SelectorItemShopListItem : SelectorListItem<Item>
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textPrice;
    [SerializeField] private Image imageIcon;
    private readonly AddressableBinding<Sprite> _bindingIcon = new();
    private CurrencyType currencyType;

    public void SetCurrencyType(CurrencyType currencyType) => this.currencyType = currencyType;

    protected override void OnBind(Item obj)
    {
        this.Selected += HandleItemSelected;

        textName.text = obj.ItemName;
        textPrice.text = obj.GetPriceBuy(currencyType).ToString();
        imageIcon.color = obj.IconColor;
        _ = SetIconAsync(obj.IconSpriteAddress);
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;

        textName.text = "";
        textPrice.text = "";
        _bindingIcon.Release();
        _bindingIcon.Cancel();
        imageIcon.sprite = null;
    }

    public void HandleItemSelected(SelectorListItem<Item> listItem)
    {
        UIEvents.RaiseBagDescriptionUpdated(listItem.Data);
    }

    public async Task SetIconAsync(string address)
    {
        imageIcon.enabled = false;
        var asset = await _bindingIcon.LoadAsync(address);
        imageIcon.sprite = asset;
        imageIcon.enabled = true;
    }

}
