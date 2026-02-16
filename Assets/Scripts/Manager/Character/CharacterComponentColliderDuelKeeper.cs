using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentColliderDuelKeeper : MonoBehaviour
{
    #region Fields

    [SerializeField] private CharacterEntityBattle characterEntityBattle;
    public CharacterEntityBattle CharacterEntityBattle => characterEntityBattle;

    #endregion

    #region Unity Lifecycle

    private void OnTriggerEnter(Collider other)
    {
        TryHandleTrigger(other);
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;    
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
    }

    private void HandleAssignCharacterToTeamBattle(
        CharacterEntityBattle characterEntityBattle, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.characterEntityBattle == characterEntityBattle)
        {
            this.gameObject.SetActive(formationCoord.Position == Position.GK);
        }
    }

    #endregion

    #region Duel Logic

    private void TryHandleTrigger(Collider other)
    {
        if (!other.CompareTag("Ball") ||
            BattleManager.Instance.IsTimeFrozen ||
            DuelManager.Instance.IsResolved ||
            DuelManager.Instance.DuelMode != DuelMode.Shoot ||
            !characterEntityBattle.CanDuel() ||
            !characterEntityBattle.IsKeeper ||
            !characterEntityBattle.IsInOwnPenaltyArea()
        )
            return;

        DuelParticipant lastDefense = DuelManager.Instance.GetLastDefense();
        DuelParticipant lastOffense = DuelManager.Instance.GetLastOffense();

        // Prevent repeat triggers and self defense
        if (lastDefense != null && lastDefense.CharacterEntityBattle == characterEntityBattle)
            return;
        if (lastOffense != null && lastOffense.CharacterEntityBattle == characterEntityBattle)
            return;

        // Prevent catching friendly fire
        if (lastOffense.CharacterEntityBattle   .IsSameTeam(characterEntityBattle))
            return;

        DuelManager.Instance.StartShootDuelCombo(characterEntityBattle, Category.Catch);
    }

    #endregion
}
