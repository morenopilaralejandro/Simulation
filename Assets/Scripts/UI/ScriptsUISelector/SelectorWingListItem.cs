using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Wing;

public class SelectorWingListItem : SelectorListItem<Wing>
{
    [Header("UI")]
    [SerializeField] private Image iconWing;
    [SerializeField] private Image iconEquipped;
    [SerializeField] private TMP_Text textName;

    protected override void OnBind(Wing obj)
    {
        this.Selected += HandleItemSelected;

        textName.text = obj.WingName;
        iconEquipped.enabled = obj.IsEquipped();
        iconWing.color = ColorManager.GetWingColor(obj.WingColorType);
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;

        textName.text = "";
        iconEquipped.enabled = false;
        iconWing.color = Color.white;
    }

    public void HandleItemSelected(SelectorListItem<Wing> listItem)
    {
        UIEvents.RaiseWingDescriptionUpdateRequested(listItem.Data);
    }

    /*
    public async Task SetIconAsync(string address)
    {
        imageIcon.enabled = false;
        var asset = await _bindingIcon.LoadAsync(address);
        imageIcon.sprite = asset;
        imageIcon.enabled = true;
    }
    */
}
