using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;

public class MenuCharacterDetailPanelMoveActions : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private Button buttonMove;
    [SerializeField] private Button buttonUnequip;
    [SerializeField] private Button buttonLimitBreak;
    [SerializeField] private Button buttonEquip;
    [SerializeField] private Button buttonBack;

    private MoveSlotUI moveSlotUI;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;
    private bool isClosing = false;
    private bool isEquiping = false;

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
        isClosing = false;

        InitializeUI();

        base.Show();
        base.SetInteractable(true);
    }

    public override void Hide()
    {
        isClosing = false;

        base.SetInteractable(false);
        base.Hide();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);

        if (interactable && isClosing)
        {
            isClosing = false;
            Close();
        }
    }

    public void Close()
    {
        if (!isTop) return;
        menuManager.CloseMenu();
    }

    #endregion

    #region Logic

    #endregion

    #region Helper

    public void InitializeUI()
    {
        bool isNull = this.moveSlotUI == null || this.moveSlotUI.Move == null;

        buttonMove.gameObject.SetActive(!isNull);
        buttonUnequip.gameObject.SetActive(!isNull);
        // buttonLimitBreak.gameObject.SetActive(!isNull);
        buttonLimitBreak.gameObject.SetActive(false);
        buttonEquip.gameObject.SetActive(isNull);
        buttonBack.gameObject.SetActive(true);

        if (isNull) 
            base.SetDefaultSelectable(buttonEquip);
        else
            base.SetDefaultSelectable(buttonMove);
    }

    #endregion

    #region Button Handle

    public void OnButtonMoveClicked() 
    {
        Close();
        UIEvents.RaiseMoveSlotUIMoveRequested(moveSlotUI);
    }

    public void OnButtonUnequipClicked() 
    {
        Close();
        UIEvents.RaiseMoveUnequipRequested(moveSlotUI.Move, moveSlotUI.Character);
    }

    public void OnButtonLimitBreakClicked() 
    {
        // open limit break menu 
        // show required item
        // show if meet evolution requirement

        Close();
        UIEvents.RaiseMoveLimitBreakPanelOpenRequested(moveSlotUI.Move, moveSlotUI.Character);
    }

    public void OnButtonEquipClicked() 
    {
        isEquiping = true;
        UIEvents.RaiseMoveSelectorOpenRequested(MoveSelectorModePopulate.GetFromLearnedExcludeEquiped, moveSlotUI.Character);
    }

    public void OnButtonBackClicked() 
    {
        Close();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnMoveActionsOpenRequested += HandleMoveActionsOpenRequested;
        UIEvents.OnMoveSelected += HandleMoveSelected;
        UIEvents.OnMoveUnequipRequested += HandleMoveUnequipRequested;
        UIEvents.OnMoveEquipRequested += HandleMoveEquipRequested;
        UIEvents.OnMoveSwapRequested += HandleMoveSwapRequested;
        UIEvents.OnBackFromMoveSelectorRequested += HandleBackFromMoveSelectorRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnMoveActionsOpenRequested -= HandleMoveActionsOpenRequested;
        UIEvents.OnMoveSelected -= HandleMoveSelected;
        UIEvents.OnMoveUnequipRequested -= HandleMoveUnequipRequested;
        UIEvents.OnMoveEquipRequested -= HandleMoveEquipRequested;
        UIEvents.OnMoveSwapRequested -= HandleMoveSwapRequested;
        UIEvents.OnBackFromMoveSelectorRequested -= HandleBackFromMoveSelectorRequested;
    }

    private void HandleMoveActionsOpenRequested(MoveSlotUI moveSlotUI) 
    {
        if (isOpen) return;
        this.moveSlotUI = moveSlotUI;
        menuManager.OpenMenu(this);
    }

    private void HandleMoveSelected(Move move)
    {
        if (!isOpen) return;
        if (!isEquiping) return;
        UIEvents.RaiseMoveEquipRequested(move, moveSlotUI.Character);
        Close();
    }

    private void HandleMoveUnequipRequested(Move move, Character character) 
    {
        character.UnequipMove(move);
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    private void HandleMoveEquipRequested(Move move, Character character) 
    {
        character.EquipMove(move);
        isEquiping = false;
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    private void HandleBackFromMoveSelectorRequested() 
    {
        isEquiping = false;
    }

    private void HandleMoveSwapRequested(Character character, int indexA, int indexB) 
    {
        character.SwapEquippedMoves(indexA, indexB);
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    #endregion
}
