using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;
using Simulation.Enums.Duel;
using Simulation.Enums.Input;

public class DuelMenu : MonoBehaviour
{
    [SerializeField] private MoveCommandSlot moveCommandSlot0;
    [SerializeField] private MoveCommandSlot moveCommandSlot1;
    [SerializeField] private GameObject panelCommand;
    [SerializeField] private GameObject panelMove;
    [SerializeField] private Button buttonCommandMelee;
    [SerializeField] private Button buttonCommandRanged;
    [SerializeField] private Button buttonCommandMove;
    [SerializeField] private Button buttonMoveNext;

    private bool isOpen;
    private bool isCommandOpen;
    private bool isMoveOpen;
    private int currentStartIndex = 0;
    private Character character;
    private Category category;
    private List<Move> moves;
    private TeamSide userSide;

    private void Awake()
    {
        BattleUIManager.Instance?.RegisterDuelMenu(this);
        Hide();

        SettingsEvents.OnAutoBattleToggled += HandleOnAutoBattleToggled;
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterDuelMenu(this);

        SettingsEvents.OnAutoBattleToggled -= HandleOnAutoBattleToggled;
    }

    private void Update() 
    {
        if (!isOpen) return;
        if (BattleUIManager.Instance.IsBattleMenuOpen) return;

        if (isCommandOpen) 
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_ClickWestButton) && 
                CanSelectRegularCommands())
                OnCommandMeleeTapped();

            if (InputManager.Instance.GetDown(CustomAction.BattleUI_ClickEastButton) && 
                CanSelectRegularCommands())
                OnCommandRangedTapped();

            if (InputManager.Instance.GetDown(CustomAction.BattleUI_ClickNorthButton) &&
                CanSelectMoveCommand())
                OnCommandMoveTapped();

        } else if (isMoveOpen)
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_ClickWestButton) &&
                moveCommandSlot0.CanBeSelected())
                OnMoveSlotTapped(moveCommandSlot0);

            if (InputManager.Instance.GetDown(CustomAction.BattleUI_ClickEastButton) && 
                moveCommandSlot1.CanBeSelected())
                OnMoveSlotTapped(moveCommandSlot1);

            if (InputManager.Instance.GetDown(CustomAction.BattleUI_ClickNorthButton) &&
                buttonMoveNext.interactable)
                OnButtonNextTapped();

            if (InputManager.Instance.GetDown(CustomAction.BattleUI_CloseMoveMenu))
                OnButtonBackTapped();

        }
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
        SetMoveButtonInteractable(
            CanSelectMoveCommand()
        );
        isOpen = true;
        this.gameObject.SetActive(true);

        if(SettingsManager.Instance.IsAutoBattleEnabled)
            DuelSelectionManager.Instance.SelectionMadeAuto(userSide);
        else
            ShowCommand();
    }

    public void Hide() 
    {
        HideMove();
        HideCommand();

        isOpen = false;    
        this.gameObject.SetActive(false);
    }

    public void ShowCommand() 
    {
        isCommandOpen = true;
        panelCommand.SetActive(true);
    }

    public void HideCommand() 
    {
        isCommandOpen = false;
        panelCommand.SetActive(false);
    }

    public void ShowMove() 
    {
        currentStartIndex = 0;
        UpdateMoveSlots();

        isMoveOpen = true;
        panelMove.SetActive(true);
    }

    public void HideMove() 
    {
        isMoveOpen = false;
        panelMove.SetActive(false);
    }

    private void SetMoveButtonInteractable(bool isInteractable) 
    {
        buttonCommandMove.interactable = isInteractable;
    }

    private bool CanSelectMoveCommand() 
    {
        return 
            DuelManager.Instance.CanSelectMoveCommand(category) && 
            moves.Count > 0;
    }

    private bool CanSelectRegularCommands() 
    {  
        return true;
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
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        DuelSelectionManager.Instance.SelectionMadeHuman(
            userSide, 
            DuelCommand.Melee, 
            null);
    }

    public void OnCommandRangedTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
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
