using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;

public class MenuCharacterDetail : Menu
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
    [Header("Other")]
    [SerializeField] private GameObject firstSelected;

    //[Header("UI References")]
    //[SerializeField] private MenuTeamMode mode;

    private Character character;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;

    private GameObject selectedGo;

    private bool isReplacingMove;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
    }

    private void OnDestroy()
    {

    }

    #endregion

   #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        selectedGo = null;
        InitializeUI();
        PopulateUI();
    }

    public override void Hide()
    {
        base.SetInteractable(false);
        base.Hide();

        selectedGo = null;
    }

    public void Close()
    {
        if (!isTop) return;
        base.SetLastSelected(firstSelected);
        menuManager.CloseMenu();
    }

    public void Refresh()
    {
        PopulateUI();
    }

    #endregion

    #region Populate

    private void InitializeUI() 
    {
        moveLayoutUI.Initialize(character);
        statLayoutUI.Initialize(character);
    }

    private void PopulateUI()
    {
        if (character == null) return;

        // basic
        characterCard.SetCharacter(character, Position.FW);
        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        barXp.SetCharacter(character);
        textLevel.text = $"{character.Level}";

        // move
        moveLayoutUI.Populate();

        // training
        statLayoutUI.Populate();

    }

    private void ClearUI()
    {
        character = null;

        characterCard.Clear();
        barHp.Clear();
        barSp.Clear();
        barXp.Clear();
        textLevel.text = "";

        moveLayoutUI.Clear();
        statLayoutUI.Clear();
    }

    #endregion

    #region Logic

    #endregion

    #region Helper


    #endregion

    #region Button Handle

    public void OnButtonBackClicked() 
    {
        Close();
    }

    public void OnButtonSelected(GameObject selectedGo)
    {
        this.selectedGo = selectedGo;
        base.SetLastSelected(selectedGo);
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnCharacterDetailOpenRequested += HandleCharacterDetailOpenRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnCharacterDetailOpenRequested -= HandleCharacterDetailOpenRequested;
    }

    private void HandleCharacterDetailOpenRequested(Character character) 
    {
        if (isOpen) return;
        this.character = character;
        menuManager.OpenMenu(this);
    }

    #endregion
}
