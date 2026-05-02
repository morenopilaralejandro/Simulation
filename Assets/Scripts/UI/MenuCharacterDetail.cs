using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

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

    private MoveSlotUI cachedMoveSlot;
    private bool isSwappingMove;

    #endregion

    #region Lifecycle

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
    }

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();
        InitializeUI();
        base.SetInteractable(true);

        selectedGo = null;
    }

    public override void Hide()
    {
        base.SetInteractable(false);
        base.Hide();

        selectedGo = null;
        isSwappingMove = false;
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);

        if (interactable)
            Refresh();

        if (interactable) 
            SubscribeInput();
        else
            UnsubscribeInput();
    }

    public void Close()
    {
        if (!isTop) return;
        base.SetLastSelected(firstSelected);
        menuManager.CloseMenu();
    }

    public void Refresh()
    {
        InitializeUI();
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

    #region Input 

    private void SubscribeInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    private void UnsubscribeInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

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
        UIEvents.OnMoveSlotUIClicked += HandleMoveSlotUIClicked;
        UIEvents.OnCharacterDetailRefreshRequested += HandleCharacterDetailRefreshRequested;
        UIEvents.OnMoveSlotUIMoveRequested += HandleMoveSlotUIMoveRequested;
        UIEvents.OnMoveSlotUIMoveCanceled += HandleMoveSlotUIMoveCanceled;
    }

    private void OnDisable()
    {
        UIEvents.OnCharacterDetailOpenRequested -= HandleCharacterDetailOpenRequested;
        UIEvents.OnMoveSlotUIClicked -= HandleMoveSlotUIClicked;
        UIEvents.OnCharacterDetailRefreshRequested -= HandleCharacterDetailRefreshRequested;
        UIEvents.OnMoveSlotUIMoveRequested -= HandleMoveSlotUIMoveRequested;
        UIEvents.OnMoveSlotUIMoveCanceled -= HandleMoveSlotUIMoveCanceled;
    }

    private void HandleCharacterDetailOpenRequested(Character character) 
    {
        if (isOpen) return;
        this.character = character;
        menuManager.OpenMenu(this);
    }

    private void HandleMoveSlotUIMoveRequested(MoveSlotUI slot) 
    {
        isSwappingMove = true;
        UIEvents.RaiseMoveSlotUIMoveStarted(slot);
    }

    private void HandleMoveSlotUIClicked(MoveSlotUI slot) 
    {
        if (!isTop) return;
        if (slot == null) return;

        if (isSwappingMove) 
        {
            isSwappingMove = false;
            UIEvents.RaiseMoveSlotUIMoveEnded(slot);
            if (slot == null || slot.Character == null) return;
            UIEvents.RaiseMoveSwapRequested(slot.Character, slot.Index, cachedMoveSlot.Index);
            return;
        }

        base.SetDefaultSelectable(slot.Button);
        cachedMoveSlot = slot;
        UIEvents.RaiseMoveActionsOpenRequested(slot);
    }

    private void HandleCharacterDetailRefreshRequested() 
    {
        Refresh();
    }

    private void HandleMoveSlotUIMoveCanceled(MoveSlotUI moveSlotUI) 
    {
        isSwappingMove = false;
        UIEvents.RaiseMoveSlotUIMoveEnded(moveSlotUI);
    }

    #endregion
}
