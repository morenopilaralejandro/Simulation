using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public class ShootDuelHandler : IDuelHandler 
{
    private Duel duel;

    public ShootDuelHandler(Duel duel) 
    { 
        this.duel = duel;
    }

    public void AddParticipant(DuelParticipant participant) 
    {
        if (duel.Participants.Count == 0) 
        {
            if (participant.Category == Category.Shoot)
            {
                PossessionManager.Instance.Release();
                BattleManager.Instance.Ball.StartTravel(
                    ShootTriangleManager.Instance.GetRandomPoint());
            }
        } else 
        {
            BattleManager.Instance.Ball.ResumeTravel();
            if (participant.Category == Category.Shoot)            
                PossessionManager.Instance.SetLastCharacter(participant.Character);
        }

        duel.Participants.Add(participant);
        LogManager.Trace($"[ShootDuelHandler] AddParticipant {participant.Character.CharacterId}");

        // Handle secret moves and SFX

        if (participant.Action == DuelAction.Offense)
        {
            duel.OffensePressure += participant.Damage;
            duel.LastOffense = participant;

            //DuelLogManager.Instance.AddActionCommand(participant.Player, participant.Command, participant.Secret);
            //DuelLogManager.Instance.AddActionDamage(participant.Action, participant.Damage);
            LogManager.Info($"[ShootDuelHandler] Offense action increases attack pressure +{participant.Damage}");
        }
        else
        {
            duel.LastDefense = participant;
            Resolve();
        }

    }

    public void Resolve()
    {
        //DuelLogManager.Instance.AddActionCommand(duel.LastDefense.Character, duel.LastDefense.Command, duel.LastDefense.Move);
        //DuelLogManager.Instance.AddActionDamage(duel.LastDefense.Action, duel.LastDefense.Damage);

        DuelManager.Instance.ApplyElementalEffectiveness(
            duel.LastOffense, 
            duel.LastDefense);

        if (duel.LastDefense.Damage >= duel.OffensePressure)
        {
            /*
            if (duel.LastDefense.Category == Category.Catch)
                AudioManager.Instance.PlaySfx("SfxCatch");
            if (duel.DuelMode == DuelMode.Shoot)
                DuelLogManager.Instance.AddActionStop(duel.LastDefense.Player);
            */
            LogManager.Info($"[ShootDuelHandler] " +
                $"{duel.LastDefense.Character.CharacterId} " +
                $"stopped the attack " +
                $"-{duel.LastDefense.Damage}");

            duel.LastOffense.Character.ApplyStatus(StatusEffect.Stunned);

            /*
                if category.block && move && move.category==shoot move.trait.block
                    Reversal
                else 
                    End
            */

            EndDuel(duel.LastDefense, duel.LastOffense);
        }
        else
        {
            duel.OffensePressure -= duel.LastDefense.Damage;

            LogManager.Info($"[ShootDuelHandler] Partial block. " +
                $"OffensePressure now {duel.OffensePressure}");

            duel.LastDefense.Character.ApplyStatus(StatusEffect.Stunned);

            if (duel.LastDefense.Category == Category.Catch)
            {
                //AudioManager.Instance.PlaySfx("SfxKeeperScream");
                LogManager.Info("[ShootDuelHandler] Keeper fails to catch the ball");
                EndDuel(duel.LastOffense, duel.LastDefense);
            }
        }
    }

    public void EndDuel(DuelParticipant winner, DuelParticipant loser) 
    { 
        //triangle
        DuelManager.Instance.EndDuel(winner, loser);
    }

    public void CancelDuel() 
    { 
        //travel
        //triangle
    }

}
