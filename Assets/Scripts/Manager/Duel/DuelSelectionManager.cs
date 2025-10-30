using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

public class DuelSelectionManager : MonoBehaviour
{
    public static DuelSelectionManager Instance { get; private set; }

    private const float SelectionTimeout = 10f;

    private DuelMode duelMode;
    private TeamSide shootDuelSelectionTeamSide;
    private Dictionary<TeamSide, DuelSelection> selections;

    public event Action OnSelectionsComplete;

    public Category GetUserCategory() =>  selections[BattleManager.Instance.GetUserSide()].Category;
    public Character GetUserCharacter() =>  selections[BattleManager.Instance.GetUserSide()].Character;

    #region Initialization
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartSelectionPhase()
    {
        duelMode = DuelManager.Instance.DuelMode;
        BattleManager.Instance.Freeze();
        BattleManager.Instance.SetBattlePhase(BattlePhase.Selection);

        DuelMode mode = DuelManager.Instance.DuelMode;

        HandleLocalSelection(mode);
    }

    public void ResetSelections()
    {
        selections =
        new Dictionary<TeamSide, DuelSelection>
        {
            { TeamSide.Home, new DuelSelection() },
            { TeamSide.Away, new DuelSelection() }
        };
    }

    public void SetShootDuelSelectionTeamSide(TeamSide teamSide) 
        => shootDuelSelectionTeamSide = teamSide;

    #endregion

    #region Local Duel Selection

    private void HandleLocalSelection(DuelMode duelMode)
    {
        this.duelMode = duelMode;
        if (duelMode == DuelMode.Field)
        {
            BattleUIManager.Instance.ShowDuelMenuForBoth();
            SelectionMadeAi();
        }
        else if (duelMode == DuelMode.Shoot)
        {
            if (shootDuelSelectionTeamSide == BattleManager.Instance.GetUserSide())
            {
                BattleUIManager.Instance.ShowDuelMenuForUser();
            }
            else
            {
                SelectionMadeAi();
            }
        }
    }

    private void SelectionMadeAi()
    {
        //get the command and move from ai script
        FinalizeSelection(TeamSide.Away, DuelCommand.Melee, null);
    }

    #endregion

    #region Selection Finalization
    public void SetPreselection(
        TeamSide teamSide, 
        Category category, 
        int participantIndex, 
        Character character) 
    {
        var selection = selections[teamSide];
        selection.ParticipantIndex = participantIndex;
        selection.Category = category;
        selection.Character = character;
    }

    public void SelectionMadeHuman(TeamSide teamSide, DuelCommand command, Move move)
    {
        if (teamSide == BattleManager.Instance.GetUserSide())
            BattleUIManager.Instance.HideDuelMenu(); 
        FinalizeSelection(teamSide, command, move);
    }

    private void FinalizeSelection(TeamSide teamSide, DuelCommand command, Move move)
    {
        var selection = selections[teamSide];
        selection.Command = command;
        selection.Move = move;
        selection.IsReady = true;

        //BattleUIManager.Instance.HideDuelMenu();
        TryFinalizeBothSelections();
    }

    private void TryFinalizeBothSelections()
    {
        bool ready;

        if (duelMode == DuelMode.Shoot)
        {
            var userSide = BattleManager.Instance.GetUserSide();
            ready = selections[userSide].IsReady;
        }
        else
        {
            ready = selections.Values.All(sel => sel.IsReady);
        }

        if (ready)
            FinalizeBothSelections();
    }

    private void FinalizeBothSelections()
    {
        foreach (var kvp in selections)
        {
            var side = kvp.Key;
            var selection = kvp.Value;
            if (duelMode == DuelMode.Shoot)
            {
                if (side == shootDuelSelectionTeamSide)
                {
                    DuelManager.Instance.RegisterSelection(
                        selection.ParticipantIndex,
                        selection.Category,
                        selection.Command,
                        selection.Move
                    );
                }
            }
            else
            {
                DuelManager.Instance.RegisterSelection(
                    selection.ParticipantIndex,
                    selection.Category,
                    selection.Command,
                    selection.Move
                );
            }
        }
        BattleManager.Instance.Unfreeze();
        BattleManager.Instance.SetBattlePhase(BattlePhase.Battle);
        OnSelectionsComplete?.Invoke();
    }

    #endregion
}
