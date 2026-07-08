using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.UI;

public class PickerAmount : Menu
{
    #region Field

    [Header("UI References")]
    [SerializeField] private TMP_Text textAmount;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textPrice;

    private PickerAmountMode mode;
    private Item item;
    private int amountCurrent;
    private int amountMin;
    private int amountMax;
    private CurrencyType currencyType;

    #endregion

    #region Overrides

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    #endregion

    #region Buttons

    public void OnButtonConfirmClicked()
    {
        switch (mode) 
        {
            case PickerAmountMode.Buy:
                ItemManager.Instance.Buy(item, amountCurrent, currencyType);
                break;
            case PickerAmountMode.Sell:
                ItemManager.Instance.Sell(item, amountCurrent, currencyType);
                break;
        }

        OnButtonCancelClicked();
        UIEvents.RaiseBagUpdated();
    }

    public void OnButtonCancelClicked()
    {
        RequestClose();
    }

    public void OnButtonAdd1() => ChangeAmount(1);
    public void OnButtonAdd10() => ChangeAmount(10);
    public void OnButtonRemove1() => ChangeAmount(-1);
    public void OnButtonRemove10() => ChangeAmount(-10);

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnPickerAmountOpened += HandleOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnPickerAmountOpened -= HandleOpened;
    }

    private void HandleOpened(PickerAmountMode mode, Item item, int min, int max, CurrencyType currencyType)
    {
        this.mode = mode;
        this.item = item;
        this.currencyType = currencyType;

        amountMin = min;
        amountMax = max;
        amountCurrent = min;

        textName.text = item.ItemName;

        switch (mode) 
        {
            case PickerAmountMode.Buy:
                textPrice.text = item.GetPriceBuy(currencyType).ToString();
                break;
            case PickerAmountMode.Sell:
                textPrice.text = item.GetPriceSell().ToString();
                break;
        }

        RefreshUI();
        MenuManager.Instance.OpenMenu(this);
    }

    #endregion

    #region Logic

    private void RefreshUI()
    {
        textAmount.text = amountCurrent.ToString();
    }

    private void ChangeAmount(int delta)
    {
        if (delta < 0 && amountCurrent == amountMin)
        {
            amountCurrent = amountMax;
        }
        else if (delta > 0 && amountCurrent == amountMax)
        {
            amountCurrent = amountMin;
        }
        else
        {
            amountCurrent = Mathf.Clamp(amountCurrent + delta, amountMin, amountMax);
        }

        RefreshUI();
    }

    #endregion

}
