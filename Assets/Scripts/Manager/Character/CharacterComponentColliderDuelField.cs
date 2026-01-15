using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentColliderDuelField : MonoBehaviour
{
    #region Fields

    [SerializeField] private Character character;
    public Character Character => character;

    #endregion

    #region Unity Lifecycle

    private void OnTriggerEnter(Collider other)
    {
        TryStartDuel(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryStartDuel(other);
    }

    #endregion

    #region Duel Logic

    private void TryStartDuel(Collider other)
    {
        if (!character.HasBall() ||
            !character.CanDuel() ||
            !other.gameObject.CompareTag(this.tag) || 
            !DuelManager.Instance.IsResolved || 
            BattleManager.Instance.IsTimeFrozen)
            return;

        Character otherCharacter = 
            other.GetComponent
                <CharacterComponentColliderDuelField>().
                Character;

        if (otherCharacter.IsSameTeam(character) ||
            !otherCharacter.CanDuel()) 
            return;

        DuelManager.Instance.StartFieldDuel(character, otherCharacter);
    }
    #endregion
}
