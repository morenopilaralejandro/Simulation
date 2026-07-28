using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Wing;

public class SelectorWingListItem : SelectorListItem<Wing>
{
    [Header("UI")]
    [SerializeField] private WingUI wingUI;

    protected override void OnBind(Wing obj)
    {
        this.Selected += HandleItemSelected;

        wingUI.SetData(obj);
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;

        wingUI.Clear();
    }

    public void HandleItemSelected(SelectorListItem<Wing> listItem)
    {
        UIEvents.RaiseWingDescriptionUpdateRequested(listItem.Data);
    }
}
