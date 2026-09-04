using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class SelectorItemShop : Selector<Item, SelectorItemShopListItem>
{
    #region Fields

    [Header("Field")]
    [SerializeField] private TMP_Text textGold;
    [SerializeField] private TMP_Text textName;
    private Shop shop;
    private int selectedIndex;

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        UIEvents.RaiseBagDescriptionUpdated(null);
        base.Show();
    }

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back, HandleBack);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorItemShopListItem view, Item data)
    {
        view.SetCurrencyType(shop.CurrencyType);
        view.Bind(data);
    }

    #endregion

    #region Public API

    #endregion

    #region Input

    private void HandleBack()
    {
        RequestClose();
        DialogEvents.RaiseDialogMenuClosed();
    }

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnItemShopSelectorOpenRequested += HandleOpenRequested;
        ItemEvents.OnCurrencyUpdated += HandleCurrencyUpdated;
        UIEvents.OnBagUpdated += HandleBagUpdated;
        UIEvents.OnItemBuyRequested += HandleItemBuyRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnItemShopSelectorOpenRequested -= HandleOpenRequested;
        ItemEvents.OnCurrencyUpdated -= HandleCurrencyUpdated;
        UIEvents.OnBagUpdated -= HandleBagUpdated;
        UIEvents.OnItemBuyRequested -= HandleItemBuyRequested;
    }

    private void HandleOpenRequested(
        ISelectorSource<Item>      source,
        ISelectorClickAction<Item> action,
        ISelectorFilter<Item>      filter)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;
    
        textGold.text = ItemManager.Instance.GetGold().ToString();

        if (source is SelectorItemShopSource s) 
        {
            shop = s.Shop;
            textName.text = s.Shop.ShopName;
        }

        Open(source, action, filter);
    }

    private void HandleCurrencyUpdated(CurrencyType currencyType, int intValue)
    {
        if(currencyType != CurrencyType.Gold) return;
        textGold.text = ItemManager.Instance.GetGold().ToString();
    }

    private void HandleBagUpdated()
    {
        if(!MenuManager.Instance.IsMenuOpen(this)) return;
        selectedIndex = GetSelectedIndex();
        Refresh();
        FocusItem(selectedIndex);
    }

    private void HandleItemBuyRequested(Item item) 
    {
        CurrencyType currencyType = this.shop.CurrencyType;
        int gold = ItemManager.Instance.GetAmount(currencyType);
        int price = item.GetPriceBuy(currencyType);

        if (price <= 0)
        {
            //Debug.LogError("Invalid item price.");
            return;
        }

        int maxAffordable = gold / price;

        if (maxAffordable <= 0)
        {
            // Not enough gold.
            return;
        }

        UIEvents.RaisePickerAmountOpened(
            PickerAmountMode.Buy,
            item,
            1,
            maxAffordable,
            currencyType
        );
    }

    #endregion
}
