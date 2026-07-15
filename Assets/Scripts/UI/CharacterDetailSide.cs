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

    [Header("Equipment")]
    [SerializeField] private EquipmentLayoutUI equipmentLayoutUI;

    [Header("Wing")]
    [SerializeField] private CanvasGroup canvasWing;
    [SerializeField] private Image wingIcon;
    [SerializeField] private TMP_Text wingTextName;
    [SerializeField] private StatLayoutUI statLayoutUIWing;

    [Header("Pages")]
    [SerializeField] private CanvasGroup pageOneCanvas;
    [SerializeField] private CanvasGroup pageTwoCanvas;
    [SerializeField] private CanvasGroup pageThreeCanvas;
    [SerializeField] private CanvasGroup pageFourCanvas;

    private Character character;
    private int page = 0;
    private int pageMax = 3;

    #endregion

    #region

    private void Awake()
    {
        SetVisible(pageOneCanvas, true);
        SetVisible(pageTwoCanvas, false);
        SetVisible(pageThreeCanvas, false);
        SetVisible(pageFourCanvas, false);

        SetVisible(canvasWing, false);
    }

    #endregion

    #region Logic

    private void PopulateUI(Character character, Position position)
    {
        this.character = character;

        if (character == null) return;

        characterCard.SetCharacter(character, position);
        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        barXp.SetCharacter(character);
        textLevel.text = $"{character.Level}";

        moveLayoutUI.Initialize(character);
        moveLayoutUI.Populate();

        statLayoutUI.Initialize(character);
        statLayoutUI.Populate();

        equipmentLayoutUI.Initialize(character);
        equipmentLayoutUI.Populate();

        if (character.HasWingEquipped)
        {
            statLayoutUIWing.Populate(character.Wing);

            wingTextName.text = character.Wing.WingName;
            wingIcon.color = ColorManager.GetWingColor(character.Wing.WingColorType);
        } else 
        {
            statLayoutUIWing.Clear();
            wingTextName.text = "";
            wingIcon.color = Color.white;
        }
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
        equipmentLayoutUI.Clear();

        statLayoutUIWing.Clear();
        wingTextName.text = "";
        wingIcon.color = Color.white;
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
        canvasGroup.alpha = isVisible ? 1f : 0f;
    }

    private void ChangePage()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");

        page++;

        if (page > pageMax)
            page = 0;

        SetVisible(pageOneCanvas, page == 0);
        SetVisible(pageTwoCanvas, page == 1);
        SetVisible(pageThreeCanvas, page == 2);
        SetVisible(pageFourCanvas, page == 3);

        SetVisible(canvasWing, page == 3);
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnCharacterDetailSideUpdateRequested += HandleCharacterDetailSideUpdateRequested;
        UIEvents.OnCharacterDetailSideNextPageRequested += HandleCharacterDetailSideNextPageRequested;
        UIEvents.OnFormationCharacterSlotUISelected += HandleFormationCharacterSlotUISelected;
    }

    private void OnDisable()
    {
        UIEvents.OnCharacterDetailSideUpdateRequested -= HandleCharacterDetailSideUpdateRequested;
        UIEvents.OnCharacterDetailSideNextPageRequested -= HandleCharacterDetailSideNextPageRequested;
        UIEvents.OnFormationCharacterSlotUISelected -= HandleFormationCharacterSlotUISelected;
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

    #endregion
}
