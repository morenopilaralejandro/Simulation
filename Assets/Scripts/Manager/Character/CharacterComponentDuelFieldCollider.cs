using UnityEngine;
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
            !other.gameObject.CompareTag(this.tag) || 
            !DuelManager.Instance.IsResolved || 
            BattleManager.Instance.IsTimeFrozen)
            return;

        Character otherCharacter = 
            other.GetComponentInParent<Character>();

        if (character.IsSameTeam(otherCharacter)) return;

        LogManager.Info(
            $"[CharacterComponentDuelFieldCollider] " +  
            $"Starting field duel between " +
            $"{character.CharacterId} ({character.TeamSide}) and " +
            $"{otherCharacter.CharacterId} ({otherCharacter.TeamSide})", this);

        bool isKeeper = 
            otherCharacter.IsKeeper &&
            otherCharacter.IsInOwnPenaltyArea();
           
        DuelManager.Instance.StartDuel(DuelMode.Field);
        DuelManager.Instance.SetIsKeeper(isKeeper);
        BattleUIManager.Instance.SetDuelParticipant(character, null);
        BattleUIManager.Instance.SetDuelParticipant(otherCharacter, null);

        Category category = Category.Dribble;
        Category otherCategory = Category.Block;

        // RegisterTrigger
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
