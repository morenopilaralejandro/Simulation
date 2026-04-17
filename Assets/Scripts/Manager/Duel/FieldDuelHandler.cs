using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;

public class FieldDuelHandler : IDuelHandler 
{
    #region Fields
    private readonly Duel duel;
    private readonly float supporterMultiplier = 0.2f;
    #endregion

    #region Contructor
    public FieldDuelHandler(Duel duel) => this.duel = duel;
    #endregion

    #region Basic Duel Logic
    public void AddParticipant(DuelParticipant participant) {

        duel.Participants.Add(participant);
        LogManager.Trace($"[FieldDuelHandler] AddParticipant {participant.CharacterEntityBattle.CharacterId}");

        if (participant.Action == DuelAction.Offense) 
            duel.LastOffense = participant;
        else
            duel.LastDefense = participant;
    
        LogParticipantAction(participant);

        if (duel.Participants.Count >= 2)
            Resolve();
    }

    public void Resolve() 
    { 
        LogManager.Trace($"[FieldDuelHandler] Start OffensePressure {duel.LastOffense.Damage}");
        LogManager.Trace($"[FieldDuelHandler] Start DefensePressure {duel.LastDefense.Damage}");

        DuelManager.Instance.ApplyElementalEffectiveness(
            duel.LastOffense, 
            duel.LastDefense);

        duel.OffensePressure = duel.LastOffense.Damage;
        duel.DefensePressure = duel.LastDefense.Damage;

        float offenseSupportDamage = GetSupportDamage(
            duel.LastOffense, 
            duel.OffenseSupports);
        float defenseSupportDamage = GetSupportDamage(
            duel.LastDefense, 
            duel.DefenseSupports);

        LogManager.Trace($"[FieldDuelHandler] offenseSupportDamage {offenseSupportDamage}");
        LogManager.Trace($"[FieldDuelHandler] defenseSupportDamage {defenseSupportDamage}");

        duel.OffensePressure += offenseSupportDamage;
        duel.DefensePressure += defenseSupportDamage;

        LogManager.Info($"[FieldDuelHandler] Final OffensePressure {duel.OffensePressure}");
        LogManager.Info($"[FieldDuelHandler] Final DefensePressure {duel.DefensePressure}");

        //UI
        /*
        BattleUIManager.Instance.SetFieldDamage(
            duel.LastOffense.CharacterEntityBattle,
            duel.OffensePressure,
            duel.LastOffense.Action);
        BattleUIManager.Instance.SetFieldDamage(
            duel.LastDefense.CharacterEntityBattle,
            duel.DefensePressure,
            duel.LastDefense.Action);
        */

        if (duel.OffensePressure > duel.DefensePressure) 
            EndDuel(duel.LastOffense, duel.LastDefense);
        else
            EndDuel(duel.LastDefense, duel.LastOffense);
    }

    public async void EndDuel(DuelParticipant winner, DuelParticipant loser) 
    { 
        if (winner.Move != null) 
        {
            winner.CharacterEntityBattle.ModifyBattleStat(Stat.Sp, -winner.Move.Cost);

            if (winner.Action == DuelAction.Offense) 
                BattleUIManager.Instance.SetDuelParticipant(winner.CharacterEntityBattle, duel.OffenseSupports);
            else
                BattleUIManager.Instance.SetDuelParticipant(winner.CharacterEntityBattle, duel.DefenseSupports);
            
            await BattleEffectManager.Instance.PlayMoveParticle(
                winner.Move,
                winner.CharacterEntityBattle.transform.position);

            MoveEvents.RaiseMoveUsed(winner.Move, winner.CharacterEntityBattle);
        }

        if (winner.CharacterEntityBattle.IsOnUsersTeam())
            BattleEffectManager.Instance.PlayDuelWinEffect(winner.CharacterEntityBattle.transform); 
        
        loser.CharacterEntityBattle.ApplyStatus(StatusEffect.Stunned);
        StunSupports(loser.Action);
        PossessionManager.Instance.GiveBallToCharacter(winner.CharacterEntityBattle);
        DuelManager.Instance.EndDuel(winner, loser);
    }

    public void CancelDuel() { }
    #endregion

    #region Support
    private float GetSupportDamage(
        DuelParticipant participant, 
        List<CharacterEntityBattle> supports) 
    {
        float damage = 0;
        int elementMatchingSupports = 1;

        foreach(CharacterEntityBattle support in supports) 
        {
            damage += DamageCalculator.GetDamage(
                participant.Category,
                participant.Command,
                support,
                null,
                false,
                false);
            if(participant.CharacterEntityBattle.Element == support.Element) 
                elementMatchingSupports++;
        }

        damage *= supporterMultiplier;
        damage *= elementMatchingSupports;

        LogManager.Trace($"[FieldDuelHandler] elementMatchingSupports {elementMatchingSupports}");
        return damage;
    }

    private void StunSupports(
        DuelAction action) 
    {
        var supports = action == DuelAction.Offense ? 
            duel.OffenseSupports :
            duel.DefenseSupports;
            
        foreach(CharacterEntityBattle support in supports) 
            support.ApplyStatus(StatusEffect.Stunned);
    }
    #endregion

    #region Helpers
    private void LogParticipantAction(DuelParticipant participant)
    {
        DuelLogManager.Instance.AddActionCommand(participant.CharacterEntityBattle, participant.Command, participant.Move);
        DuelLogManager.Instance.AddActionDamage(participant.CharacterEntityBattle, participant.Action, participant.Damage);
    }
    #endregion

}
