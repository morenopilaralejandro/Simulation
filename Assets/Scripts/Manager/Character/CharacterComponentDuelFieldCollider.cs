using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentDuelFieldCollider : MonoBehaviour
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
            other.GetComponentInParent<Character>();

        if (otherCharacter.IsSameTeam(character) ||
            !otherCharacter.CanDuel()) 
            return;

        LogManager.Info(
            $"[CharacterComponentDuelFieldCollider] " +  
            $"Starting field duel between " +
            $"{character.CharacterId} ({character.TeamSide}) and " +
            $"{otherCharacter.CharacterId} ({otherCharacter.TeamSide})", this);

        bool isKeeperDuel = 
            otherCharacter.IsKeeper &&
            otherCharacter.IsInOwnPenaltyArea();
           
        DuelManager.Instance.StartDuel(DuelMode.Field);
        DuelManager.Instance.SetIsKeeperDuel(isKeeperDuel);

        Category category = Category.Dribble;
        Category otherCategory = Category.Block;

        //Support
        List<Character> offenseSupports =
            DuelManager.Instance.FindNearbySupporters(character);
        List<Character> defenseSupports =
            DuelManager.Instance.FindNearbySupporters(otherCharacter);
        
        DuelManager.Instance.SetOffenseSupports(offenseSupports);
        DuelManager.Instance.SetDefenseSupports(defenseSupports);
        //UI
        BattleUIManager.Instance.SetDuelParticipant(character, offenseSupports);
        BattleUIManager.Instance.SetDuelParticipant(otherCharacter, defenseSupports);
        //RegisterTrigger
        DuelManager.Instance.RegisterTrigger(
            character, 
            false);
        DuelManager.Instance.RegisterTrigger(
            otherCharacter, 
            false);
        //SetPreselection
        DuelSelectionManager.Instance.SetPreselection(
            character.TeamSide, 
            category, 
            0, 
            character);
        DuelSelectionManager.Instance.SetPreselection(
            otherCharacter.TeamSide, 
            otherCategory, 
            1, 
            otherCharacter);
            
        DuelSelectionManager.Instance.StartSelectionPhase();
    }

    /*
    private bool CanStartDuel()
    {
        return true;        
        return Time.time >= _nextDuelAllowedTime
               && !GameManager.Instance.IsMovementFrozen
               && DuelManager.Instance.IsDuelResolved();
        
    }
    */

    #endregion
}
