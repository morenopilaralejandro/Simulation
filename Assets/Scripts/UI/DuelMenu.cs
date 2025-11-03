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
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterDuelMenu(this);
    }

    private void Update() 
    {
        if (!isOpen) return;

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
        moves = character.GetEquippedMovesByCategory(category);
        userSide = BattleManager.Instance.GetUserSide();
    }

    public void Show() 
    {
        SetCharacter();
        SetMoveButtonInteractable(
            CanSelectMoveCommand()
        );
        isOpen = true;
        this.gameObject.SetActive(true);

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
            //DuelManager.Instance.CanSelectMoveCommand(category) && 
            moves.Count > 0;
    }

    private bool CanSelectRegularCommands() 
    {  
        return true;
    }

    public void OnButtonBackTapped()
    {
        HideMove();
        ShowCommand();
    }

    public void OnButtonNextTapped()
    {
        currentStartIndex = (currentStartIndex + 2) % moves.Count;
        UpdateMoveSlots();
    }

    public void OnCommandMeleeTapped()
    {
        //AudioManager.Instance.PlaySfx("SfxMenuTap");
        DuelSelectionManager.Instance.SelectionMadeHuman(
            userSide, 
            DuelCommand.Melee, 
            null);
    }

    public void OnCommandRangedTapped()
    {
        //AudioManager.Instance.PlaySfx("SfxMenuTap");
        DuelSelectionManager.Instance.SelectionMadeHuman(
            userSide, 
            DuelCommand.Ranged, 
            null);
    }

    public void OnCommandMoveTapped()
    {
        //AudioManager.Instance.PlaySfx("SfxSecretCommand");
        HideCommand();
        ShowMove();
    }

    private void UpdateMoveSlots()
    {
        if (moves == null || moves.Count == 0) return;

        int index0 = currentStartIndex % moves.Count;
        moveCommandSlot0.SetMove(moves[index0]);
        moveCommandSlot0.SetInteractable(character.CanAffordMove(moves[index0]));

        if (moves.Count > 1)
        {
            moveCommandSlot1.SetInteractable(true);
            int index1 = (currentStartIndex + 1) % moves.Count;
            moveCommandSlot1.SetMove(moves[index1]);
            buttonMoveNext.interactable = true;
            moveCommandSlot1.SetInteractable(character.CanAffordMove(moves[index1]));
        }
        else
        {
            // Only one move — disable 2nd + next button
            buttonMoveNext.interactable = false;
            moveCommandSlot1.SetActive(false);
        }

    }

    public void OnMoveSlotTapped(MoveCommandSlot moveCommandSlot)
    {
        if (moveCommandSlot?.Move == null) return;

        /*
        if (localSel.Player.GetStat(PlayerStats.Sp) < secretCommandSlot.Secret.Cost)
        {
            AudioManager.Instance.PlaySfx("SfxForbidden");
            return;
        }
        */

        //AudioManager.Instance.PlaySfx("SfxSecretSelect");

        DuelSelectionManager.Instance.SelectionMadeHuman(
            userSide, 
            DuelCommand.Move, 
            moveCommandSlot.Move);
    }

}
