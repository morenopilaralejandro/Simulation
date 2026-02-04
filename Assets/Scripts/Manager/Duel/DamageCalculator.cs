using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public static class DamageCalculator
{
    // Multiplier constants
    private const float MAIN_MULTIPLIER = 0.5f;
    private const float SUB_MULTIPLIER = 0.25f;
    private const float MOVE_MULTIPLIER = 2f;
    private const float ELEMENT_MATCH_MULTIPLIER = 1.5f;
    public const float ELEMENT_EFFECTIVE_MULTIPLIER = 1.5f;

    private const float KEEPER_MULTIPLIER = 1.5f;

    private const float DISTANCE_MULTIPLIER = 10f;
    private const float DIRECT_BONUS = 50f;


    // Helper function for Melee/Ranged formulas
    private static float CalcFormula(Character character, Stat main, Stat sub0, Stat sub1)
    {
        return
            character.GetBattleStat(main) * MAIN_MULTIPLIER +
            character.GetBattleStat(sub0) * SUB_MULTIPLIER +
            character.GetBattleStat(sub1) * SUB_MULTIPLIER +
            character.GetBattleStat(Stat.Courage) * SUB_MULTIPLIER;
    }

    // Helper function for Move formulas
    private static float CalcMove(Character character, Move move, Stat main)
    {
        if (move == null) return 0f;
        float baseDamage =
            move.Power * MOVE_MULTIPLIER +
            character.GetBattleStat(main) * MAIN_MULTIPLIER +
            character.GetBattleStat(Stat.Courage) * MAIN_MULTIPLIER;
        if (character.Element == move.Element)
            baseDamage *= ELEMENT_MATCH_MULTIPLIER;
        return baseDamage;
    }

    // Special for Shoot (minus distance)
    private static float CalcDistanceReduction(Character character)
    {
        return GoalManager.Instance.GetDistanceToOpponentGoal(character) * DISTANCE_MULTIPLIER;
    }

    public static Dictionary<(Category, DuelCommand), Func<Character, Move, float>> damageFormulas =
        new Dictionary<(Category, DuelCommand), Func<Character, Move, float>>()
    {
        // Dribble
        {(Category.Dribble, DuelCommand.Melee),  (character, move) => CalcFormula(character, Stat.Control, Stat.Body, Stat.Stamina)},
        {(Category.Dribble, DuelCommand.Ranged), (character, move) => CalcFormula(character, Stat.Control, Stat.Kick, Stat.Speed)},
        {(Category.Dribble, DuelCommand.Move), (character, move) => CalcMove(character, move, Stat.Control)},

        // Block
        {(Category.Block, DuelCommand.Melee),    (character, move) => CalcFormula(character, Stat.Body, Stat.Guard, Stat.Stamina)},
        {(Category.Block, DuelCommand.Ranged),   (character, move) => CalcFormula(character, Stat.Body, Stat.Control, Stat.Speed)},
        {(Category.Block, DuelCommand.Move),  (character, move) => CalcMove(character, move, Stat.Body)},

        // Shoot
        {(Category.Shoot, DuelCommand.Melee),    (character, move) => CalcFormula(character, Stat.Kick, Stat.Body, Stat.Stamina)},
        {(Category.Shoot, DuelCommand.Ranged),   (character, move) => CalcFormula(character, Stat.Kick, Stat.Control, Stat.Speed)},
        {(Category.Shoot, DuelCommand.Move),  (character, move) => CalcMove(character, move, Stat.Kick)},

        // Catch
        {(Category.Catch, DuelCommand.Melee),    (character, move) => CalcFormula(character, Stat.Guard, Stat.Body, Stat.Stamina)},
        {(Category.Catch, DuelCommand.Ranged),   (character, move) => CalcFormula(character, Stat.Guard, Stat.Control, Stat.Speed)},
        {(Category.Catch, DuelCommand.Move),  (character, move) => CalcMove(character, move, Stat.Guard)}
    };

    public static float GetDamage(
        Category category, 
        DuelCommand command, 
        Character character, 
        Move move,
        bool isKeeperDuel,
        bool isDirect)
    {
        float damage = 0f;
        if (damageFormulas.TryGetValue((category, command), out var formula))
            damage = formula(character, move);

        if (isKeeperDuel && character.IsKeeper)
            damage *= KEEPER_MULTIPLIER;

        bool appliesDistancePenalty =
            category == Category.Shoot &&
            move != null &&
            (move.Trait == Trait.Long || move.Trait == Trait.Block);

        if (appliesDistancePenalty)
            damage -= CalcDistanceReduction(character);

        if (isDirect)
            damage += DIRECT_BONUS;

        return damage;
    }

    public static bool IsEffective(
        Element offenseElement, 
        Element defenseElement)
    {
        int offenseIndex = (int)offenseElement;
        int defenseIndex = (int)defenseElement;

        // Compute next in cycle (with wrap-around)
        int nextIndex = (offenseIndex + 1) % System.Enum.GetValues(typeof(Element)).Length;

        return defenseIndex == nextIndex;
    }
}
