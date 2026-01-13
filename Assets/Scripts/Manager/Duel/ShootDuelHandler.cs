using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public class ShootDuelHandler : IDuelHandler 
{
    #region Fields
    private readonly Duel duel;
    #endregion

    #region Contructor
    public ShootDuelHandler(Duel duel) => this.duel = duel;
    #endregion
    
    #region Basic Duel Logic
    public void EndDuel(DuelParticipant winner, DuelParticipant loser) => DuelManager.Instance.EndDuel(winner, loser);

    public void CancelDuel() { }

    public void AddParticipant(DuelParticipant participant) 
    {
        bool isFirstParticipant = duel.Participants.Count == 0;
        bool isCategoryShoot = participant.Category == Category.Shoot;
        bool isActionOffense = participant.Action == DuelAction.Offense;

        if (isFirstParticipant)
            StartBallTravel(participant);
        else if (isCategoryShoot) 
            PossessionManager.Instance.SetLastCharacter(participant.Character); //keep track of the last character in the shoot chain to determine who scored

        duel.Participants.Add(participant);
        LogManager.Trace($"[ShootDuelHandler] AddParticipant {participant.Character.CharacterId}");

        ProcessParticipantAction(participant, isActionOffense);
    }

    private void ProcessParticipantAction(DuelParticipant participant, bool isActionOffense)
    {
        HandleMove(participant);

        if (isActionOffense)
        {
            duel.LastOffense = participant;
            HandleOffense(participant);
        }
        else
        {
            duel.LastDefense = participant;
            Resolve();
        }
    }
    #endregion

    #region Offense Logic
    private void HandleOffense(DuelParticipant offense) 
    {
        bool isFirstParticipant = duel.Participants.Count == 1;

        duel.OffensePressure += offense.Damage;
        LogParticipantAction(offense);

        BattleManager.Instance.Ball.UpdateTravelEffect(offense.Move, offense.CurrentElement);
        HandleShootSfx(offense);

        if (isFirstParticipant) 
        {
            DuelManager.Instance.StartBallTravel(offense);
        } else 
        {
            PossessionManager.Instance.SetLastCharacter(offense.Character); //keep track of the last character in the shoot chain to determine who scored
            BattleManager.Instance.Ball.ResumeTravel();
        }

    }
    #endregion

    #region Defense Logic
    public void Resolve()
    {
        DuelParticipant offense = duel.LastOffense;
        DuelParticipant defense = duel.LastDefense;

        DuelManager.Instance.ApplyElementalEffectiveness(offense, defense);

        duel.OffensePressure -= defense.Damage;
        LogParticipantAction(defense);

        bool isCategoryCatch = defense.Category == Category.Catch;
        if (duel.OffensePressure <= 0.9)
            HandleDefenseFull(offense, defense, isCategoryCatch);
        else
            HandleDefensePartial(offense, defense, isCategoryCatch);
    }

    private void HandleDefenseFull(DuelParticipant offense, DuelParticipant defense, bool isCategoryCatch)
    {
        bool isShootReversal = defense.Move?.Category == Category.Shoot && DuelManager.Instance.IsShootReversalAllowed;

        LogManager.Info($"[ShootDuelHandler] {defense.Character.CharacterId} stopped the attack.");

        BattleEvents.RaiseShootStopped(defense.Character);
        offense.Character.ApplyStatus(StatusEffect.Stunned);
        PossessionManager.Instance.GiveBallToCharacter(defense.Character);

        // if is reversal start else end
        if (isShootReversal) 
        {
            DuelManager.Instance.StartShootDuelReversal();
        } else 
        {
            EndDuel(defense, offense);
            BattleManager.Instance.Ball.EndTravel();
        }
    }

    private void HandleDefensePartial(DuelParticipant offense, DuelParticipant defense, bool isCategoryCatch)
    {
        LogManager.Info($"[ShootDuelHandler] Partial block.");

        defense.Character.ApplyStatus(StatusEffect.Stunned);

        BattleManager.Instance.Ball.ResumeTravel();

        if (isCategoryCatch)
        {
            LogManager.Info("[ShootDuelHandler] Keeper fails to catch the ball");
            EndDuel(offense, defense);
        }
    }
    #endregion

    #region Helpers
    private void StartBallTravel(DuelParticipant participant)
    {
        PossessionManager.Instance.Release();
        BattleManager.Instance.Ball.StartTravel(
            ShootTriangleManager.Instance.GetRandomPoint(),
            participant.Command);
    }

    private void HandleMove(DuelParticipant participant) 
    {
        if (participant.Move == null) return;

        participant.Character.ModifyBattleStat(Stat.Sp, -participant.Move.Cost);
        BattleUIManager.Instance.SetDuelParticipant(participant.Character, null);
    }

    private void HandleShootSfx(DuelParticipant participant) 
    {
        if (participant.Move != null) 
            AudioManager.Instance.PlaySfx("sfx-ball_shoot_regular");    
        else 
            AudioManager.Instance.PlaySfx("sfx-ball_shoot_special");
    }

    private void LogParticipantAction(DuelParticipant participant)
    {
        bool isActionOffense = participant.Action == DuelAction.Offense;

        DuelLogManager.Instance.AddActionCommand(participant.Character, participant.Command, participant.Move);
        DuelLogManager.Instance.AddActionDamage(participant.Character, participant.Action, participant.Damage);
        BattleUIManager.Instance.SetComboDamage(duel.OffensePressure);

        if(isActionOffense)
            LogManager.Info($"[ShootDuelHandler] Offense action increases attack pressure +{participant.Damage}");
        else
            LogManager.Info($"[ShootDuelHandler] Defense action decreases attack pressure -{participant.Damage}");

        LogManager.Info($"[ShootDuelHandler] OffensePressure now {duel.OffensePressure}");
    }
    #endregion

}
