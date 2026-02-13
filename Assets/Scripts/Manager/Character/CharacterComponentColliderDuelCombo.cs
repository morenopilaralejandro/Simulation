using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentColliderDuelCombo : MonoBehaviour
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

    #endregion

    #region Duel Logic

    private void TryHandleTrigger(Collider other)
    {
        if (!other.CompareTag("Ball") || 
            BattleManager.Instance.IsTimeFrozen || 
            DuelManager.Instance.IsResolved || 
            DuelManager.Instance.DuelMode != DuelMode.Shoot ||
            !characterEntityBattle.CanDuel() ||
            (characterEntityBattle.IsKeeper && characterEntityBattle.IsInOwnPenaltyArea())
        )
            return;

        DuelParticipant lastOffense = DuelManager.Instance.GetLastOffense();
        DuelParticipant lastDefense = DuelManager.Instance.GetLastDefense();

        //Prevent repeat triggers by the same defense player
        if (lastDefense != null && lastDefense.CharacterEntityBattle == characterEntityBattle)
            return;

        if (lastOffense == null || lastOffense.CharacterEntityBattle == characterEntityBattle)
            return;

        Category category = PossessionManager.Instance.LastCharacter.IsSameTeam(characterEntityBattle) ? 
            Category.Shoot : 
            Category.Block;

        DuelManager.Instance.StartShootDuelCombo(characterEntityBattle, category);
    }

    #endregion
}
