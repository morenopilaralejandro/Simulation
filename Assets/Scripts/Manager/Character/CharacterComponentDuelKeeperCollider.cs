using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentDuelKeeperCollider : MonoBehaviour
{
    #region Fields
    private Character character;
    #endregion

    #region Unity Lifecycle
    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleTrigger(other);
    }
    #endregion

    #region Duel Logic
    private void TryHandleTrigger(Collider other)
    {
        if (!other.CompareTag("Ball") ||
            BattleManager.Instance.IsTimeFrozen ||
            DuelManager.Instance.IsResolved ||
            DuelManager.Instance.DuelMode != DuelMode.Shoot ||
            !character.CanDuel() ||
            !character.IsKeeper ||
            !character.IsInOwnPenaltyArea()
        )
            return;

        DuelParticipant lastDefense = DuelManager.Instance.GetLastDefense();
        DuelParticipant lastOffense = DuelManager.Instance.GetLastOffense();

        // Prevent repeat triggers and self defense
        if (lastDefense != null && lastDefense.Character == character)
            return;
        if (lastOffense != null && lastOffense.Character == character)
            return;

        // Prevent catching friendly fire
        if (lastOffense.Character.IsSameTeam(character))
            return;

        int participantIndex = DuelManager.Instance.GetParticipantCount();

        LogManager.Info(
            $"[CharacterComponentDuelKeeperCollider] " +  
            $"Registering trigger for " +
            $"{character.CharacterId} ({character.TeamSide}), " +
            $"participantIndex {participantIndex}", this);

        //UI
        //BattleUIManager.Instance.SetDuelParticipant(character, null);
        
        //Travel
        //BallTravelController.Instance.PauseTravel();

        //RegisterTrigger
        DuelManager.Instance.RegisterTrigger(character, false);
        //SetPreselection
        DuelSelectionManager.Instance.SetPreselection(
            character.TeamSide, 
            Category.Catch, 
            participantIndex, 
            character);
        DuelSelectionManager.Instance.SetShootDuelSelectionTeamSide(
            character.TeamSide);
        DuelSelectionManager.Instance.StartSelectionPhase();
    }
    #endregion
}
