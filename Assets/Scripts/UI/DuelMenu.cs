using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Input;

public class DuelMenu : MonoBehaviour
{
    [SerializeField] private MoveCommandSlot moveCommandSlot0;
    [SerializeField] private MoveCommandSlot moveCommandSlot1;
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private CanvasGroup panelCommand;
    [SerializeField] private CanvasGroup panelMove;
    [SerializeField] private Button buttonCommandMelee;
    [SerializeField] private Button buttonCommandRanged;
    [SerializeField] private Button buttonCommandMove;
    [SerializeField] private Button buttonMoveNext;

    private bool isOpen;
    private bool isCommandOpen;
    private bool isMoveOpen;
    private int currentStartIndex = 0;
    private CharacterEntityBattle character;
    private Category category;
    private List<Move> moves;
    private TeamSide userSide;
    private InputManager inputManager;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        BattleUIManager.Instance?.RegisterDuelMenu(this);
        Hide();

        SettingsEvents.OnAutoBattleToggled += HandleOnAutoBattleToggled;
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterDuelMenu(this);

        SettingsEvents.OnAutoBattleToggled -= HandleOnAutoBattleToggled;
        UnsubscribeInput();
    }

    private void SubscribeInput()
    {
        inputManager.SubscribeDown(CustomAction.BattleUI_ClickWestButton, HandleWest);
        inputManager.SubscribeDown(CustomAction.BattleUI_ClickEastButton, HandleEast);
        inputManager.SubscribeDown(CustomAction.BattleUI_ClickNorthButton, HandleNorth);
        inputManager.SubscribeDown(CustomAction.BattleUI_CloseMoveMenu, HandleSouth);
    }

    private void UnsubscribeInput()
    {
        inputManager.UnsubscribeDown(CustomAction.BattleUI_ClickWestButton, HandleWest);
        inputManager.UnsubscribeDown(CustomAction.BattleUI_ClickEastButton, HandleEast);
        inputManager.UnsubscribeDown(CustomAction.BattleUI_ClickNorthButton, HandleNorth);
        inputManager.UnsubscribeDown(CustomAction.BattleUI_CloseMoveMenu, HandleSouth);
    }

    private void HandleWest() 
    {
        if (isCommandOpen && CanSelectRegularCommands()) 
            OnCommandMeleeTapped();
        else if (isMoveOpen && moveCommandSlot0.CanBeSelected())
            OnMoveSlotTapped(moveCommandSlot0);
    }

    private void HandleEast() 
    {
        if (isCommandOpen && CanSelectRegularCommands()) 
            OnCommandRangedTapped();
        else if (isMoveOpen && moveCommandSlot1.CanBeSelected())
            OnMoveSlotTapped(moveCommandSlot1);
    }

    private void HandleNorth() 
    { 
        if (isCommandOpen && CanSelectMoveCommand()) 
            OnCommandMoveTapped();
        else if (isMoveOpen && buttonMoveNext.interactable)
            OnButtonNextTapped();
    }

    private void HandleSouth() 
    { 
        if (isMoveOpen)
            OnButtonBackTapped();
    }

    private void SetCharacter() 
    {
        category = DuelSelectionManager.Instance.GetUserCategory();
        character = DuelSelectionManager.Instance.GetUserCharacter();
        userSide = BattleManager.Instance.GetUserSide();

        Trait? requiredTrait = DuelManager.Instance.GetRequiredTraitByCategory(category);
        if (requiredTrait.HasValue) 
            moves = character.GetEquippedMovesByTrait(requiredTrait.Value);
        else 
            moves = character.GetEquippedMovesByCategory(category);

        BattleUIManager.Instance.SetDuelCategory(category);
    }

    public void Show()
    {
        SetCharacter();

        SetMoveButtonInteractable(CanSelectMoveCommand());
        SetRegularButtonsInteractable(CanSelectRegularCommands());

        isOpen = true;

        SetCanvasGroup(rootCanvasGroup, true);

        SubscribeInput();

        if (SettingsManager.Instance.IsAutoBattleEnabled)
            DuelSelectionManager.Instance.SelectionMadeAuto(userSide);
        else
            ShowCommand();
    }

    public void Hide()
    {
        HideMove();
        HideCommand();

        UnsubscribeInput();

        isOpen = false;

        SetCanvasGroup(rootCanvasGroup, false);
    }

    private static void SetCanvasGroup(CanvasGroup cg, bool visible)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }

    public void ShowCommand()
    {
        isCommandOpen = true;
        SetCanvasGroup(panelCommand, true);
    }

    public void HideCommand()
    {
        isCommandOpen = false;
        SetCanvasGroup(panelCommand, false);
    }

    public void ShowMove()
    {
        currentStartIndex = 0;
        UpdateMoveSlots();

        isMoveOpen = true;
        SetCanvasGroup(panelMove, true);
    }

    public void HideMove()
    {
        isMoveOpen = false;
        SetCanvasGroup(panelMove, false);
    }

    private void SetMoveButtonInteractable(bool isInteractable) 
    {
        buttonCommandMove.interactable = isInteractable;
    }

    private void SetRegularButtonsInteractable(bool isInteractable) 
    {
        buttonCommandMelee.interactable = isInteractable;
        buttonCommandRanged.interactable = isInteractable;
    }

    private bool CanSelectMoveCommand() 
    {
        return 
            DuelManager.Instance.CanSelectMoveCommand(category) && 
            moves.Count > 0;
    }

    private bool CanSelectRegularCommands() 
    {  
        return DuelManager.Instance.CanSelectRegularCommands();
    }

    public void OnButtonBackTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_back");
        HideMove();
        ShowCommand();
    }

    public void OnButtonNextTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu-move-command");

        currentStartIndex += 2;

        // Prevent wrapping — stop when we exceed move count
        if (currentStartIndex >= moves.Count)
            currentStartIndex = 0;

        UpdateMoveSlots();
    }

    public void OnCommandMeleeTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu-regular-command");
        DuelSelectionManager.Instance.SelectionMadeHuman(
            userSide, 
            DuelCommand.Melee, 
            null);
    }

    public void OnCommandRangedTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu-regular-command");
        DuelSelectionManager.Instance.SelectionMadeHuman(
            userSide, 
            DuelCommand.Ranged, 
            null);
    }

    public void OnCommandMoveTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu-move-command");
        HideCommand();
        ShowMove();
    }

    private void UpdateMoveSlots()
    {
        if (moves == null || moves.Count == 0)
            return;

        // Always show first move slot
        moveCommandSlot0.SetActive(true);
        moveCommandSlot0.SetMove(moves[currentStartIndex]);
        moveCommandSlot0.SetInteractable(character.CanAffordMove(moves[currentStartIndex]));

        int nextIndex = currentStartIndex + 1;

        // If there's another move in range, show it.
        if (nextIndex < moves.Count)
        {
            moveCommandSlot1.SetActive(true);
            moveCommandSlot1.SetMove(moves[nextIndex]);
            moveCommandSlot1.SetInteractable(character.CanAffordMove(moves[nextIndex]));
        }
        else
        {
            // Hide second slot if no more moves
            moveCommandSlot1.SetActive(false);
        }

        // Enable/disable “Next” button only if there are more moves ahead
        buttonMoveNext.interactable = (currentStartIndex + 2 < moves.Count);
    }

    public void OnMoveSlotTapped(MoveCommandSlot moveCommandSlot)
    {
        if (moveCommandSlot?.Move == null) return;
        AudioManager.Instance.PlaySfx("sfx-menu-move-select");

        /*
        if (localSel.Player.GetStat(PlayerStats.Sp) < secretCommandSlot.Secret.Cost)
        {
            AudioManager.Instance.PlaySfx("SfxForbidden");
            return;
        }
        */

        DuelSelectionManager.Instance.SelectionMadeHuman(
            userSide, 
            DuelCommand.Move, 
            moveCommandSlot.Move);
    }

    private void HandleOnAutoBattleToggled(bool enable) 
    {
        if (isOpen && enable) 
        {
            DuelSelectionManager.Instance.SelectionMadeAuto(userSide);
            Hide();
        }
    }

}
