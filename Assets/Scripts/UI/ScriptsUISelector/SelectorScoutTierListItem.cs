using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Scout;

public class SelectorScoutTierListItem : SelectorListItem<ScoutTier>
{
    [Header("Character UI")]
    [SerializeField] private TMP_Text textName;

    protected override void OnBind(ScoutTier obj)
    {
        textName.text = obj.ScoutTierName;
    }

    protected override void OnUnbind()
    {
        textName.text = "";
    }
}
