using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class SelectorLoadoutListItem : SelectorListItem<Team>
{
    [Header("UI Elements")]
    [SerializeField] private Image imageEmblem;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject activeIndicator;

    protected override void OnBind(Team data)
    {
        imageEmblem.sprite = data.TeamCrestSprite;
        nameText.text = data.TeamName;

        bool isActive = TeamManager.Instance.ActiveLoadoutGuid == data.TeamGuid;
        activeIndicator.SetActive(isActive);
    }

    protected override void OnUnbind()
    {
        imageEmblem.sprite = null;
        nameText.text = "";
    }
}
