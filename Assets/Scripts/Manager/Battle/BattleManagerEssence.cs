using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Move;

public class BattleManagerEssence
{
    #region Fields

    private float factorFieldDuel = 0.20f;
    private float factorShootDuelBlock = 0.15f;
    private float factorShootDuelCatch = 0.15f;
    private float factorShootDuelShoot = 0.20f;
    private int minimumDamage = 1;

    private readonly Dictionary<TeamSide, int> essenceOverflowUnderwent = new Dictionary<TeamSide, int>();
    private int essenceOverflowLimit = 99;
    private int essenceOverflowLimitFull = 5;
    private int essenceOverflowLimitMini = 3;

    #endregion

    #region Constructor

    public BattleManagerEssence() { }

    public void Reset()
    {
        essenceOverflowUnderwent[TeamSide.Home] = 0;
        essenceOverflowUnderwent[TeamSide.Away] = 0;
    }

    public void Initialize(BattleType battleType)
    {
        Reset();

        essenceOverflowLimit = GetOverflowLimit(battleType);

        foreach (Team team in BattleManager.Instance.Teams.Values)
        {
            foreach (Character character in team.GetCharacters(battleType)) 
            {
                if(character.IsFainted)
                    essenceOverflowUnderwent[team.TeamSide]++;
            }
        }
    }

    #endregion

    #region Logic

    public void ApplyEssenceDamage(
        DuelParticipant winner,
        DuelParticipant loser,
        DuelMode duelMode,
        float offensePressure,
        bool isPunching)
    {
        // Punching moves prevent essence damage to the goalkeeper
        if (isPunching) return;

        int damage = CalculateEssenceDamage(
            winner,
            loser,
            duelMode,
            offensePressure,
            isPunching);

        loser.CharacterEntityBattle.ModifyBattleStat(Stat.Hp, -damage);
        LogManager.Trace($"[BattleManagerEssence] {loser.CharacterEntityBattle.CharacterId} lost {damage} HP");

        bool fainted = loser.CharacterEntityBattle.GetBattleStat(Stat.Hp) <= 0;
        if (fainted) HandleFaint(loser.CharacterEntityBattle);
    }

    private void HandleFaint(CharacterEntityBattle characterEntityBattle)
    {
        characterEntityBattle.SetStatusPermanent(StatusEffectPermanent.Fainted);
        CharacterChangeControlManager.Instance.TryChangeOnEssence(characterEntityBattle);
        characterEntityBattle.gameObject.SetActive(false);
        TeamSide teamSide = characterEntityBattle.TeamSide;

        essenceOverflowUnderwent[teamSide]++;

        EssenceEvents.RaiseCharacterUnderwentEssenceOverflow(characterEntityBattle);
        EssenceEvents.RaisePlayEssenceVfxRequested(characterEntityBattle.transform);


        if (HasBattleEnded(teamSide))
            EssenceEvents.RaiseEssenceBattleLimitReached(teamSide, essenceOverflowUnderwent[teamSide]);
    }


    #endregion

    #region Helpers

    private int CalculateEssenceDamage(
        DuelParticipant winner,
        DuelParticipant loser,
        DuelMode duelMode,
        float offensePressure,
        bool isPunching)
    {
        float baseDamage = GetBaseDamage(winner, loser, duelMode, offensePressure);

        float factor = GetFactor(duelMode, loser);

        int stamina = loser.CharacterEntityBattle.GetBattleStat(Stat.Stamina);
        int hp = loser.CharacterEntityBattle.GetBattleStat(Stat.Hp);

        /*
         * Stamina acts as a damage resistance.
         * At high stamina: damage is reduced.
         * At low stamina: damage approaches the full calculated amount.
         * We use: staminaResistance = stamina / (stamina + 100)
         *
         * This gives diminishing returns and avoids extremely
         * high-stamina characters becoming completely immune.
         */

        float staminaResistance = stamina / (float)(stamina + 100);

        float staminaMultiplier = 1f - staminaResistance;

        float damage = baseDamage * factor * staminaMultiplier;

        // Never allow essence damage to exceed current HP.
        //damage = Mathf.Min(damage, hp);

        return Mathf.Max(minimumDamage, Mathf.RoundToInt(damage));
    }

    private float GetBaseDamage(
        DuelParticipant winner,
        DuelParticipant loser,
        DuelMode duelMode,
        float offensePressure)
    {
        if (loser.Category == Category.Block && duelMode == DuelMode.Shoot) 
            return offensePressure;
        else
            return winner.Damage;
    }

    private float GetFactor(
        DuelMode duelMode,
        DuelParticipant loser)
    {
        switch (duelMode)
        {
            case DuelMode.Field:
                return factorFieldDuel;
            case DuelMode.Shoot:
                switch (loser.Category)
                {
                    case Category.Catch:
                        return factorShootDuelCatch;
                    case Category.Shoot:
                        return factorShootDuelShoot;
                    default:
                        return factorShootDuelBlock;
                }
            default:
                return factorFieldDuel;
        }
    }

    private int GetOverflowLimit(BattleType battleType)
    {
        if (battleType == BattleType.Full)
            return essenceOverflowLimitFull;
        else 
            return essenceOverflowLimitMini;
    }

    public bool HasBattleEnded(TeamSide teamSide) => essenceOverflowUnderwent[teamSide] >= essenceOverflowLimit;

    #endregion

    #region Events

    public void Subscribe() 
    {
        BattleEvents.OnBattleStarted += HandleBattleStarted;
        EssenceEvents.OnCharacterEssenceOverflowRequested += HandleCharacterEssenceOverflowRequested;
    }

    public void Unsubscribe() 
    { 
        BattleEvents.OnBattleStarted -= HandleBattleStarted;
        EssenceEvents.OnCharacterEssenceOverflowRequested -= HandleCharacterEssenceOverflowRequested;
    }

    private void HandleBattleStarted(BattleType battleType) 
    {
        Initialize(battleType);
    }

    private void HandleCharacterEssenceOverflowRequested(CharacterEntityBattle entity) 
    {
        HandleFaint(entity);
    }

    #endregion
}
