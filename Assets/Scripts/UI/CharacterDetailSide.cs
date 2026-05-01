using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;

public class CharacterDetailSide : MonoBehaviour
{
    #region Fields

    [Header("Basic")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private BarHPSP barHp;
    [SerializeField] private BarHPSP barSp;
    [SerializeField] private BarXP barXp;
    [SerializeField] private TMP_Text textLevel;
    [Header("Moves")]
    [SerializeField] private MoveLayoutUI moveLayoutUI;
    [Header("Stats")]
    [SerializeField] private StatLayoutUI statLayoutUI;
    [Header("Pages")]
    [SerializeField] private CanvasGroup pageOneCanvas;
    [SerializeField] private CanvasGroup pageTwoCanvas;

    private Character character;
    private int page = 0;
    private int pageMax = 1;

    #endregion

    #region

    private void Awake() 
    {
        SetVisible(pageOneCanvas, true);
        SetVisible(pageTwoCanvas, false);
    }

    #endregion

    #region Logic

    private void PopulateUI(Character character, Position position)
    {
        this.character = character;

        if(character == null) return;

        characterCard.SetCharacter(character, Position.FW);
        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        barXp.SetCharacter(character);
        textLevel.text = $"{character.Level}";

        moveLayoutUI.Initialize(character);
        moveLayoutUI.Populate();
        statLayoutUI.Initialize(character);
        statLayoutUI.Populate();
    }

    public void Clear()
    {
        this.character = null;

        characterCard.Clear();
        barHp.Clear();
        barSp.Clear();
        barXp.Clear();
        textLevel.text = "";

        moveLayoutUI.Clear();
        statLayoutUI.Clear();
    }

    #endregion

    #region Button

    public void OnButtonNextClicked() 
    {
        UIEvents.RaiseCharacterDetailSideNextPageRequested();
    }

    public void OnButtonDetailClicked() 
    {
        if (character == null) return;
        UIEvents.RaiseCharacterDetailOpenRequested(character);
    }

    #endregion

    #region Pagination

    private void SetVisible(CanvasGroup canvasGroup, bool isVisible)
    {
        if (isVisible)
            canvasGroup.alpha = 1f;
        else
            canvasGroup.alpha = 0f;
    }

    private void ChangePage() 
    {
        page++;

        if (page > pageMax)
            page = 0;

        switch(page) 
        {
            case 0:
                SetVisible(pageTwoCanvas, false);
                SetVisible(pageOneCanvas, true);
                break;
            case 1:
                SetVisible(pageOneCanvas, false);
                SetVisible(pageTwoCanvas, true);
                break;
        }
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnCharacterDetailSideUpdateRequested += HandleCharacterDetailSideUpdateRequested;
        UIEvents.OnCharacterDetailSideNextPageRequested += HandleCharacterDetailSideNextPageRequested;
        UIEvents.OnFormationCharacterSlotUISelected += HandleFormationCharacterSlotUISelected;
        UIEvents.OnFormationCharacterSlotUIHighlighted += HandleFormationCharacterSlotUIHighlighted;
        UIEvents.OnFormationCharacterSlotUIClicked += HandleFormationCharacterSlotUIClicked;
    }

    private void OnDisable()
    {
        UIEvents.OnCharacterDetailSideUpdateRequested -= HandleCharacterDetailSideUpdateRequested;
        UIEvents.OnCharacterDetailSideNextPageRequested -= HandleCharacterDetailSideNextPageRequested;
        UIEvents.OnFormationCharacterSlotUISelected -= HandleFormationCharacterSlotUISelected;
        UIEvents.OnFormationCharacterSlotUIHighlighted -= HandleFormationCharacterSlotUIHighlighted;
        UIEvents.OnFormationCharacterSlotUIClicked -= HandleFormationCharacterSlotUIClicked;
    }

    private void HandleCharacterDetailSideUpdateRequested(Character character, Position position) 
    {
        PopulateUI(character, position);
    }

    private void HandleCharacterDetailSideNextPageRequested() 
    {
        ChangePage();
    }

    private void HandleFormationCharacterSlotUISelected(FormationCharacterSlotUI slot) 
    {
        UIEvents.RaiseCharacterDetailSideUpdateRequested(slot.GetCharacter(), slot.FormationCoord.Position);
    }

    private void HandleFormationCharacterSlotUIHighlighted(FormationCharacterSlotUI slot) 
    {
        UIEvents.RaiseCharacterDetailSideUpdateRequested(slot.GetCharacter(), slot.FormationCoord.Position);
    }

    private void HandleFormationCharacterSlotUIClicked(FormationCharacterSlotUI slot) 
    {
        UIEvents.RaiseCharacterDetailSideUpdateRequested(slot.GetCharacter(), slot.FormationCoord.Position);
    }

    #endregion

}
