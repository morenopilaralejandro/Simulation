using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Scout;

public class SelectorScoutEntryListItem : SelectorListItem<ScoutEntry>
{
    [Header("Character UI")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private TMP_Text textCost;
    [SerializeField] private TMP_Text textLv;
    [SerializeField] private CanvasGroup canvasGroupCost;
    [SerializeField] private CanvasGroup canvasGroupOwned;

    public Character Character;

    protected override void OnBind(ScoutEntry obj)
    {
        this.Selected += HandleItemSelected;

        characterCard.SetCharacter(obj.Character, obj.Character.Position);
        textCost.text = obj.Cost.ToString();
        textLv.text = obj.Level.ToString();

        textCost.color = obj.IsAffordable ? Color.white : Color.red;

        Character = obj.Character;

        SetCanvasGroup(canvasGroupOwned, obj.IsOwned);
        SetCanvasGroup(canvasGroupCost, !obj.IsOwned);
    }

    protected override void OnUnbind()
    {
        this.Selected -= HandleItemSelected;

        characterCard.Clear();
        textCost.text = "";
        textLv.text = "";

        Character = null;

        SetCanvasGroup(canvasGroupOwned, false);
        SetCanvasGroup(canvasGroupCost, false);
    }

    public void HandleItemSelected(SelectorListItem<ScoutEntry> listItem)
    {
        UIEvents.RaiseCharacterDetailSideUpdateRequested(
            listItem.Data.Character,
            listItem.Data.Character.Position);
    }

    private void SetCanvasGroup(CanvasGroup canvasGroup, bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
