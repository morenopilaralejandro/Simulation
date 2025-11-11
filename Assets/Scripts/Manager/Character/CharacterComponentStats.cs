using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentStats : MonoBehaviour
{
    //trainedStats and freedom are used in training component
    private Character character;

    [SerializeField] private Dictionary<Stat, int> baseStats = new();       //from character data
    [SerializeField] private Dictionary<Stat, int> trainedStats = new();    //gained from training
    [SerializeField] private Dictionary<Stat, int> trueStats = new();       //calculated after scaling baseStats
    [SerializeField] private Dictionary<Stat, int> battleStats = new();     //used during battle

    [Range(0f, 1f)] private float minStatRatioHp = 0.4f; // 40% of base at level 1
    [Range(0f, 1f)] private float minStatRatioSp = 0.4f; // 40% of base at level 1
    [Range(0f, 1f)] private float minStatRatioOther = 0.1f; // 10% of base at level 1

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            int baseStatValue = GetBaseStatValueFromData(stat, characterData);
            baseStats[stat] = baseStatValue;
            trainedStats[stat] = 0;
            trueStats[stat] = 0;
            battleStats[stat] = 0;
        }

        UpdateStats();
    }

    private int GetBaseStatValueFromData(Stat stat, CharacterData characterData)
    {
        return stat switch
        {
            Stat.Hp => characterData.Hp,
            Stat.Sp => characterData.Sp,
            Stat.Kick => characterData.Kick,
            Stat.Body => characterData.Body,
            Stat.Control => characterData.Control,
            Stat.Guard => characterData.Guard,
            Stat.Speed => characterData.Speed,
            Stat.Stamina => characterData.Stamina,
            Stat.Technique => characterData.Technique,
            Stat.Luck => characterData.Luck,
            Stat.Courage => characterData.Courage,
            _ => 0
        };
    }

    public int GetTrainedStat(Stat stat) => trainedStats[stat];
    public int GetTrueStat(Stat stat) => trueStats[stat];
    public int GetBattleStat(Stat stat) => battleStats[stat];

    public void ResetBattleStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            battleStats[stat] = trueStats[stat];
        }
    }

    public void ModifyBattleStat(Stat stat, int amount)
    {
        if (stat == Stat.Hp && amount < 0) 
            amount = GetReducedHpAmount(amount);
        battleStats[stat] = Mathf.Clamp(battleStats[stat] + amount, 0, trueStats[stat]);
        if (stat == Stat.Hp) this.character.UpdateFatigue();
    }

    public void ModifyTrainedStat(Stat stat, int amount)
    {
        trainedStats[stat] = Mathf.Clamp(trainedStats[stat] + amount, 0, this.character.MaxTrainingPerStat);
    }

    public void ResetTrainedStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
            trainedStats[stat] = 0;
    }

    public void UpdateStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
        {
            trueStats[stat] = ScaleStat(baseStats[stat], stat) + trainedStats[stat];
            battleStats[stat] = trueStats[stat];   
        }
    }

    private int ScaleStat(int baseStat, Stat stat)
    {
        float t = (float)(this.character.Level - 1) / (this.character.MaxLevel - 1);

        // HP and SP - Linear scaling
        if (stat == Stat.Hp)
        {
            float minRatio = minStatRatioHp; // Example: 0.4f for 40 at lvl 1 if baseStat is 100
            float value = baseStat * Mathf.Lerp(minRatio, 1f, t);
            return Mathf.RoundToInt(value);
        }
        if (stat == Stat.Sp)
        {
            float minRatio = minStatRatioSp;
            float value = baseStat * Mathf.Lerp(minRatio, 1f, t);
            return Mathf.RoundToInt(value);
        }

        // Other stats - Quadratic scaling
        {
            float minRatio = minStatRatioOther; // Example: 0.1f for 10 at lvl 1 if baseStat is 100
            // Quadratic interpolation between minRatio and 1, more curve!
            float q = t * t; // Quadratic interpolation (t squared)
            float value = baseStat * Mathf.Lerp(minRatio, 1f, q);
            return Mathf.RoundToInt(value);
        }
    }

    private int GetReducedHpAmount(int amount)
    {
        //amount is negative
        const float LvReductionPerLevel = 0.01f;
        const float MaxLvReduction = 0.7f;
        const float StaminaDivisor = 130f;
        const float MaxStaminaReduction = 0.3f;
        const float MinDamageTaken = -1f;

        float stamina = battleStats[Stat.Stamina];

        // Calculate reduction factors
        float lvFactor = 1f - Mathf.Min(this.character.Level * LvReductionPerLevel, MaxLvReduction);
        float staminaFactor = 1f - Mathf.Min(stamina / StaminaDivisor, MaxStaminaReduction);

        // Combine both (multiplicative, so boosts "stack" in reducing damage)
        float totalFactor = lvFactor * staminaFactor;

        // Ensure damage never goes below a minimum (e.g., at least 1)
        float damageTaken = Mathf.Min(MinDamageTaken, amount * totalFactor);

        return (int)damageTaken;
    }

}
