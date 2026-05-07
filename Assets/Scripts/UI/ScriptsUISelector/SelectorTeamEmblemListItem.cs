using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectorTeamEmblemListItem : SelectorListItem<SelectorTeamEmblemData>
{
    [Header("UI Elements")] 
    [SerializeField] private Image emblemImage;

    protected override void OnBind(SelectorTeamEmblemData data)
    {
        emblemImage.sprite = data.EmblemSprite;
    }

    protected override void OnUnbind()
    {
        emblemImage.sprite = null;
    }
}
