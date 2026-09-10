using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Wing;

public class SelectorWingListItem : SelectorListItem<Wing>
{
    [Header("UI")]
    [SerializeField] private WingUI wingUI;
    private Wing wing;

    protected override void OnBind(Wing obj)
    {
        this.Selected += HandleItemSelected;
        LimitBreakEvents.OnWingLimitBreakPerformed += HandleWingLimitBreakPerformed;

        this.wing = obj;
        wingUI.SetData(obj);
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;
        LimitBreakEvents.OnWingLimitBreakPerformed -= HandleWingLimitBreakPerformed;

        wingUI.Clear();
    }

    public void HandleItemSelected(SelectorListItem<Wing> listItem)
    {
        UIEvents.RaiseWingDescriptionUpdateRequested(listItem.Data);
    }

    public void HandleWingLimitBreakPerformed(Wing wing)
    {
        if (this.wing.WingGuid == wing.WingGuid)
            wingUI.SetData(wing);
    }
}
