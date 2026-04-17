using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentColliderDuelField : MonoBehaviour
{
    #region Fields

    [SerializeField] private CharacterEntityBattle character;
    public CharacterEntityBattle CharacterEntityBattle => character;

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

        CharacterEntityBattle otherCharacter = 
            other.GetComponent
                <CharacterComponentColliderDuelField>().
                CharacterEntityBattle;

        if (otherCharacter.IsSameTeam(character) ||
            !otherCharacter.CanDuel()) 
            return;

        DuelManager.Instance.StartFieldDuel(character, otherCharacter);
    }
    #endregion
}
