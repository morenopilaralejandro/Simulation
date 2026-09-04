using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.World;

public class SelectorFastTravelPointListItem : SelectorListItem<FastTravelPoint>
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;

    protected override void OnBind(FastTravelPoint obj)
    {
        textName.text = obj.ZoneName;
    }

    protected override void OnUnbind()
    {
        textName.text = "";
    }
}
