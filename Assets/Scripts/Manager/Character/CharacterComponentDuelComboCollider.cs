using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentDuelComboCollider : MonoBehaviour
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
            (character.IsKeeper && character.IsInOwnPenaltyArea())
        )
            return;

        DuelParticipant lastOffense = DuelManager.Instance.GetLastOffense();
        DuelParticipant lastDefense = DuelManager.Instance.GetLastDefense();

        //Prevent repeat triggers by the same defense player
        if (lastDefense != null && lastDefense.Character == character)
            return;

        if (lastOffense == null || lastOffense.Character == character)
            return;

        Category category = PossessionManager.Instance.LastCharacter.IsSameTeam(character) ? 
            Category.Shoot : 
            Category.Block;

        DuelManager.Instance.StartShootDuelCombo(character, category);
    }
    #endregion
}
