using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Move;

public class BattleComponentEndGame
{
    #region Fields

    private bool pendingEndByEssence;
    private TeamSide pendingTeamSide;

    #endregion

    #region Constructor

    public BattleComponentEndGame() { }

    public void Reset()
    {
        pendingEndByEssence = false;
    }

    #endregion

    #region Logic

    #endregion

    #region Events

    public void Subscribe() 
    {
        EssenceEvents.OnEssenceBattleLimitReached += HandleEssenceBattleLimitReached;
        DuelEvents.OnDuelEnded += HandleDuelEnded;
    }

    public void Unsubscribe() 
    { 
        EssenceEvents.OnEssenceBattleLimitReached -= HandleEssenceBattleLimitReached;
        DuelEvents.OnDuelEnded -= HandleDuelEnded;
    }

    private void HandleEssenceBattleLimitReached(TeamSide teamSide, int timesUnderwent) 
    {
        if(DuelManager.Instance.IsResolved)
        {
            BattleManager.Instance.EndBattleByEssence(teamSide);
        }
        else 
        {
            pendingEndByEssence = true;
            pendingTeamSide = teamSide;
        }
    }

    private void HandleDuelEnded(
        DuelMode duelMode,
        DuelParticipant winner, 
        DuelParticipant loser,
        bool isWinnerUser)
    {
        if (!pendingEndByEssence) return;

        BattleManager.Instance.EndBattleByEssence(pendingTeamSide);
        pendingEndByEssence = false;
    }

    #endregion
}
