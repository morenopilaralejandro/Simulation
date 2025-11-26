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
                    ShootTriangleManager.Instance.GetRandomPoint(),
                    participant.Command
                );
            }
        } else 
        {
            if (participant.Category == Category.Shoot)            
                PossessionManager.Instance.SetLastCharacter(participant.Character);
        }

        duel.Participants.Add(participant);
        LogManager.Trace($"[ShootDuelHandler] AddParticipant {participant.Character.CharacterId}");

        // Handle secret moves and SFX
        if (participant.Move != null) 
        {
            participant.Character.ModifyBattleStat(Stat.Sp, -participant.Move.Cost);
            BattleUIManager.Instance.SetDuelParticipant(participant.Character, null);
        }

        if (participant.Category == Category.Shoot) 
        {
            if (participant.Move != null) 
            {
                AudioManager.Instance.PlaySfx("sfx-ball_shoot_regular");
            } else 
            {
                AudioManager.Instance.PlaySfx("sfx-ball_shoot_special");
            }
        }
            

        if (participant.Action == DuelAction.Offense)
        {
            duel.OffensePressure += participant.Damage;
            duel.LastOffense = participant;

            BattleManager.Instance.Ball.ResumeTravel();

            DuelLogManager.Instance.AddActionCommand(participant.Character, participant.Command, participant.Move);
            //DuelLogManager.Instance.AddActionDamage(participant.Action, participant.Damage);
            LogManager.Info($"[ShootDuelHandler] " + 
                $"Offense action increases attack pressure " +
                $"+{participant.Damage}");
        }
        else
        {
            duel.LastDefense = participant;
            Resolve();
        }

    }

    public void Resolve()
    {
        DuelLogManager.Instance.AddActionCommand(duel.LastDefense.Character, duel.LastDefense.Command, duel.LastDefense.Move);
        //DuelLogManager.Instance.AddActionDamage(duel.LastDefense.Action, duel.LastDefense.Damage);

        DuelManager.Instance.ApplyElementalEffectiveness(
            duel.LastOffense, 
            duel.LastDefense);

        LogManager.Info($"[ShootDuelHandler] " +
            $"Defense action decreases attack pressure " +
            $"-{duel.LastDefense.Damage}");
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
                $"stopped the attack. " +
                $"OffensePressure now {duel.OffensePressure}");

            duel.LastOffense.Character.ApplyStatus(StatusEffect.Stunned);

            BattleManager.Instance.Ball.EndTravel();
            PossessionManager.Instance.GiveBallToCharacter(duel.LastDefense.Character);

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
            //duel.LastDefense.Character.gameObject.SetActive(false);

            BattleManager.Instance.Ball.ResumeTravel();

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
        DuelManager.Instance.EndDuel(winner, loser);
    }

    public void CancelDuel() 
    { 

    }

}
