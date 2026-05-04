using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuCharacterDetailPanelMoveActions : Menu
{
    [Header("UI References")]
    [SerializeField] private Button buttonMove;
    [SerializeField] private Button buttonUnequip;
    [SerializeField] private Button buttonLimitBreak;
    [SerializeField] private Button buttonEquip;
    [SerializeField] private Button buttonBack;

    private MoveSlotUI moveSlotUI;
    private bool isEquiping = false;

    private void InitializeUI()
    {
        bool isNull = moveSlotUI == null || moveSlotUI.Move == null;

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

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonMoveClicked()
    {
        RequestClose();
        UIEvents.RaiseMoveSlotUIMoveRequested(moveSlotUI);
    }

    public void OnButtonUnequipClicked()
    {
        RequestClose();
        UIEvents.RaiseMoveUnequipRequested(moveSlotUI.Move, moveSlotUI.Character);
    }

    public void OnButtonLimitBreakClicked()
    {
        RequestClose();
        UIEvents.RaiseMoveLimitBreakPanelOpenRequested(moveSlotUI.Move, moveSlotUI.Character);
    }

    public void OnButtonEquipClicked()
    {
        isEquiping = true;
        UIEvents.RaiseMoveSelectorOpenRequested(MoveSelectorModePopulate.GetFromLearnedExcludeEquiped, moveSlotUI.Character);
    }

    public void OnButtonBackClicked()
    {
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMoveActionsOpenRequested += HandleMoveActionsOpenRequested;
        UIEvents.OnMoveSelected += HandleMoveSelected;
        UIEvents.OnMoveUnequipRequested += HandleMoveUnequipRequested;
        UIEvents.OnMoveEquipRequested += HandleMoveEquipRequested;
        UIEvents.OnMoveSwapRequested += HandleMoveSwapRequested;
        UIEvents.OnBackFromMoveSelectorRequested += HandleBackFromMoveSelectorRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMoveActionsOpenRequested -= HandleMoveActionsOpenRequested;
        UIEvents.OnMoveSelected -= HandleMoveSelected;
        UIEvents.OnMoveUnequipRequested -= HandleMoveUnequipRequested;
        UIEvents.OnMoveEquipRequested -= HandleMoveEquipRequested;
        UIEvents.OnMoveSwapRequested -= HandleMoveSwapRequested;
        UIEvents.OnBackFromMoveSelectorRequested -= HandleBackFromMoveSelectorRequested;
    }

    private void HandleMoveActionsOpenRequested(MoveSlotUI moveSlotUI)
    {
        this.moveSlotUI = moveSlotUI;
        InitializeUI();
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleMoveSelected(Move move)
    {
        if (!isEquiping) return;
        UIEvents.RaiseMoveEquipRequested(move, moveSlotUI.Character);
        isEquiping = false;
        RequestClose();
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

}
