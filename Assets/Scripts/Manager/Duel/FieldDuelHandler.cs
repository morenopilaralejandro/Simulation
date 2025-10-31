using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public class FieldDuelHandler : IDuelHandler 
{
    private Duel duel;
    private float supporterMultiplier = 0.2f;

    public FieldDuelHandler(Duel duel) 
    { 
        this.duel = duel;
    }

    public void AddParticipant(DuelParticipant participant) {
        duel.Participants.Add(participant);
        LogManager.Trace($"[FieldDuelHandler] AddParticipant {participant.Character.CharacterId}");
        if (participant.Action == DuelAction.Offense) 
        {
            duel.LastOffense = participant;
        } else {
            duel.LastDefense = participant;
        }

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

        if (duel.OffensePressure > duel.DefensePressure) 
        {
            EndDuel(duel.LastOffense, duel.LastDefense);
        } else {
            EndDuel(duel.LastDefense, duel.LastOffense);
        }
    }

    public void EndDuel(DuelParticipant winner, DuelParticipant loser) 
    { 
        loser.Character.ApplyStatus(StatusEffect.Stunned);
        StunSupports(loser.Action);
        PossessionManager.Instance.GiveBallToCharacter(winner.Character);
        DuelManager.Instance.EndDuel(winner, loser);
    }

    public void CancelDuel() { }

    private float GetSupportDamage(
        DuelParticipant participant, 
        List<Character> supports) 
    {
        float damage = 0;
        int elementMatchingSupports = 1;

        foreach(Character support in supports) 
        {
            damage += DamageCalculator.GetDamage(
                participant.Category,
                participant.Command,
                support,
                null,
                false,
                false);
            if(participant.Character.Element == support.Element) 
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
            
        foreach(Character support in supports) 
            support.ApplyStatus(StatusEffect.Stunned);
    }

}
