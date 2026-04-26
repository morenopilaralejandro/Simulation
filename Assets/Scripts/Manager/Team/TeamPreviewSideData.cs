using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Team;

public class TeamPreviewSideData
{
    public TeamSide Side { get; }
    public Team OwnTeam { get; }
    public Team OpponentTeam { get; }
    public BattleType BattleType { get; private set; }
    public TeamPreviewSideState State { get; private set; }
    public int ConfirmCount { get; private set; }
    public bool IsReady => State == TeamPreviewSideState.Ready;

    private const int TotalPages = 2;

    public TeamPreviewSideData(TeamSide side, Team ownTeam, Team opponentTeam, BattleType battleType)
    {
        Side = side;
        OwnTeam = ownTeam;
        OpponentTeam = opponentTeam;
        BattleType = battleType;
        State = TeamPreviewSideState.ShowingOwnTeam;
        ConfirmCount = 0;
    }

    public void Confirm()
    {
        if (IsReady) return;

        ConfirmCount++;

        switch (ConfirmCount)
        {
            case 1:
                SetState(TeamPreviewSideState.ShowingOpponentTeam);
                TeamEvents.RaiseTeamPreviewPageChanged(Side, OpponentTeam, BattleType, 2, TotalPages);
                break;

            case 2:
                SetState(TeamPreviewSideState.Ready);
                TeamEvents.RaiseTeamPreviewSideReady(Side);
                break;
        }
    }

    public void ShowFirstPage()
    {
        SetState(TeamPreviewSideState.ShowingOwnTeam);
        TeamEvents.RaiseTeamPreviewPageChanged(Side, OwnTeam, BattleType, 1, TotalPages);
    }

    public void Reset()
    {
        State = TeamPreviewSideState.ShowingOwnTeam;
        ConfirmCount = 0;
    }

    private void SetState(TeamPreviewSideState newState)
    {
        State = newState;
        TeamEvents.RaiseTeamPreviewSideStateChanged(Side, newState);
    }
}
