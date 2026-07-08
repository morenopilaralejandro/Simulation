using UnityEngine;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class SelectorItemShopAction : ISelectorClickAction<Item>
{
    public void Execute(Item obj, IClosableMenu menu)
    {
        UIEvents.RaiseItemBuyRequested(obj);
    }
}
