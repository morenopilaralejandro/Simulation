using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Move;

public class CharacterComponentAIDuelCommand
{
    #region Field

    private CharacterEntityBattle character;

    #endregion

    #region Lifecycle

    public CharacterComponentAIDuelCommand(CharacterEntityBattle characterEntityBattle)
    {
        this.character = characterEntityBattle;
    }

    #endregion

    #region Logic

    public DuelCommand GetCommandByCategory(Category category)
    {
        if (DuelManager.Instance.IsKeeperDuel && category == Category.Dribble)
            return GetRegularCommand();

        switch (character.AIDifficulty)
        {
            case AIDifficulty.Easy:
                return GetRegularCommand();
            case AIDifficulty.Normal:
                if (Random.value < 0.4f && character.HasAffordableMoveWithCategory(category))
                    return DuelCommand.Move;
                return GetRegularCommand();
            case AIDifficulty.Hard:
                return character.HasAffordableMoveWithCategory(category)
                    ? DuelCommand.Move
                    : GetRegularCommand();
            default:
                return DuelCommand.Melee;
        }
    }

    public DuelCommand GetCommandByTrait(Trait trait)
    {
        switch (character.AIDifficulty)
        {
            case AIDifficulty.Easy:
                return GetRegularCommand();
            case AIDifficulty.Normal:
                if (Random.value < 0.4f && character.HasAffordableMoveWithTrait(trait))
                    return DuelCommand.Move;
                return GetRegularCommand();
            case AIDifficulty.Hard:
                return character.HasAffordableMoveWithTrait(trait)
                    ? DuelCommand.Move
                    : GetRegularCommand();
            default:
                return DuelCommand.Melee;
        }
    }

    public DuelCommand GetRegularCommand() =>
        character.GetBattleStat(Stat.Body) > character.GetBattleStat(Stat.Control)
            ? DuelCommand.Melee
            : DuelCommand.Ranged;

    public Move GetMoveByCommandAndCategory(DuelCommand command, Category category)
    {
        if (command != DuelCommand.Move) return null;
        return (character.AIDifficulty == AIDifficulty.Normal)
            ? character.GetRandomAffordableMoveByCategory(category)
            : character.GetStrongestAffordableMoveByCategory(category);
    }

    public Move GetMoveByCommandAndTrait(DuelCommand command, Trait trait)
    {
        if (command != DuelCommand.Move) return null;
        return (character.AIDifficulty == AIDifficulty.Normal)
            ? character.GetRandomAffordableMoveByTrait(trait)
            : character.GetStrongestAffordableMoveByTrait(trait);
    }

    public bool ShouldActivateWings()
    {
        float chance = character.FormationCoord.Position switch
        {
            Position.FW => 0.60f,
            Position.MF => 0.10f,
            Position.DF => 0.05f,
            Position.GK => 0.50f,
            _ => 0f
        };

        return UnityEngine.Random.value < chance;
    }

    #endregion
}
