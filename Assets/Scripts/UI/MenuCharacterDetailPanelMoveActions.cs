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
    private bool isEquipping = false;
    private SelectorMoveSourceFromLearnedExcludeEquipped selectorSource = new SelectorMoveSourceFromLearnedExcludeEquipped();

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
        AudioManager.Instance.PlaySfxUI("sfx-menu_change");
        RequestClose();
        UIEvents.RaiseMoveActionsCloseRequested(moveSlotUI);
        UIEvents.RaiseMoveSlotUIMoveRequested(moveSlotUI);
    }

    public void OnButtonUnequipClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        RequestClose();
        UIEvents.RaiseMoveActionsCloseRequested(moveSlotUI);
        UIEvents.RaiseMoveUnequipRequested(moveSlotUI.Move, moveSlotUI.Character);
    }

    public void OnButtonLimitBreakClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        RequestClose();
        UIEvents.RaiseMoveActionsCloseRequested(moveSlotUI);
        UIEvents.RaiseMoveLimitBreakPanelOpenRequested(moveSlotUI.Move, moveSlotUI.Character);
    }

    public void OnButtonEquipClicked()
    {
        isEquipping = true;
        selectorSource.SetCharacter(moveSlotUI.Character);
        UIEvents.RaiseMoveSelectorOpenRequested(
            selectorSource,
            new SelectorMoveAction(),
            null
        );
    }

    public void OnButtonBackClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_back");
        RequestClose();
        UIEvents.RaiseMoveActionsCloseRequested(moveSlotUI);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMoveActionsOpenRequested += HandleMoveActionsOpenRequested;
        UIEvents.OnSelectorMoveActionClicked += HandleSelectorMoveActionClicked;
        UIEvents.OnMoveUnequipRequested += HandleMoveUnequipRequested;
        UIEvents.OnMoveEquipRequested += HandleMoveEquipRequested;
        UIEvents.OnMoveSwapRequested += HandleMoveSwapRequested;
        UIEvents.OnBackFromMoveSelectorRequested += HandleBackFromMoveSelectorRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMoveActionsOpenRequested -= HandleMoveActionsOpenRequested;
        UIEvents.OnSelectorMoveActionClicked -= HandleSelectorMoveActionClicked;
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

    private void HandleSelectorMoveActionClicked(Move move)
    {
        if (!isEquipping) return;
        UIEvents.RaiseMoveEquipRequested(move, moveSlotUI.Character);
        isEquipping = false;
    }

    private void HandleMoveUnequipRequested(Move move, Character character)
    {
        character.UnequipMove(move);
        UIEvents.RaiseCharacterDetailRefreshRequested();
        RequestClose();
        UIEvents.RaiseMoveActionsCloseRequested(moveSlotUI);
    }

    private void HandleMoveEquipRequested(Move move, Character character)
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        character.EquipMove(move);
        isEquipping = false;
        UIEvents.RaiseCharacterDetailRefreshRequested();
        RequestClose();
        UIEvents.RaiseMoveActionsCloseRequested(moveSlotUI);
    }

    private void HandleBackFromMoveSelectorRequested()
    {
        isEquipping = false;
    }

    private void HandleMoveSwapRequested(Character character, int indexA, int indexB)
    {
        character.SwapEquippedMoves(indexA, indexB);
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

}
