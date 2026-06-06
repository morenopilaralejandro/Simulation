using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingComponentStats
{
    private Wing wing;

    private Dictionary<Stat, int> baseStats = new();
    private Dictionary<Stat, int> individualStats = new();
    private Dictionary<Stat, int> trueStats = new();
    //private int MaxIndividualPerStat = 31;
    private float factorIndividual = 0.5f; 
    private float factorEvolution = 0.1f;

    //[Range(0f, 1f)] private float minStatRatioOther = 0.1f; // 10% of base at level 1

    public WingComponentStats(WingData wingData, Wing wing, WingSaveData wingSaveData = null) 
    {
        Initialize(wingData, wing, wingSaveData);
    }

    public void Initialize(WingData wingData, Wing wing, WingSaveData wingSaveData = null) 
    {
        this.wing = wing;

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            if (stat == Stat.Hp || stat == Stat.Sp) continue;
            int baseStatValue = GetBaseStatValueFromData(stat, wingData);
            baseStats[stat] = baseStatValue;
            int individualStatValue = GetIndividualStatValueFromData(stat, wingData);
            individualStats[stat] = individualStatValue;
            trueStats[stat] = 0;
        }

        if (wingSaveData != null) 
        {
            foreach (WingStatSaveData wingStatSaveData in wingSaveData.IndividualStats) 
                SetIndividualStat(wingStatSaveData.Stat, wingStatSaveData.Val);
        }

        UpdateStats();
    }

    private int GetBaseStatValueFromData(Stat stat, WingData wingData)
    {
        return stat switch
        {
            Stat.Kick => wingData.KickBase,
            Stat.Body => wingData.BodyBase,
            Stat.Control => wingData.ControlBase,
            Stat.Guard => wingData.GuardBase,
            Stat.Speed => wingData.SpeedBase,
            Stat.Stamina => wingData.StaminaBase,
            Stat.Technique => wingData.TechniqueBase,
            Stat.Luck => wingData.LuckBase,
            Stat.Courage => wingData.CourageBase,
            _ => 0
        };
    }

    private int GetIndividualStatValueFromData(Stat stat, WingData wingData)
    {
        return stat switch
        {
            Stat.Kick => wingData.KickIndividual,
            Stat.Body => wingData.BodyIndividual,
            Stat.Control => wingData.ControlIndividual,
            Stat.Guard => wingData.GuardIndividual,
            Stat.Speed => wingData.SpeedIndividual,
            Stat.Stamina => wingData.StaminaIndividual,
            Stat.Technique => wingData.TechniqueIndividual,
            Stat.Luck => wingData.LuckIndividual,
            Stat.Courage => wingData.CourageIndividual,
            _ => 0
        };
    }

    public int GetIndividualStat(Stat stat) => individualStats[stat];
    public int GetTrueStat(Stat stat) => trueStats[stat];

    public void SetIndividualStat(Stat stat, int amount)
    {
        individualStats[stat] = amount;
    }

    public void UpdateStats()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
        {
            if (stat == Stat.Hp || stat == Stat.Sp) continue;
            trueStats[stat] = ScaleStat(stat);
        }
    }

    private int ScaleStat(Stat stat)
    {
        float calculatedValue = baseStats[stat] + 
            (individualStats[stat] * factorIndividual) *
            (((wing.CurrentEvolutionIndex + 1) * factorEvolution) + 1);

        int roundedResult = (int)MathF.Round(calculatedValue, MidpointRounding.AwayFromZero);

        return roundedResult;
    }
}
