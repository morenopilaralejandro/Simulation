using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingComponentRefinement
{
    private Wing wing;

    public WingRefinementRank CurrentRank { get; private set; }
    public int CurrentRankIndex => (int)CurrentRank;
    public int CurrentRankProgress { get; private set; }

    public WingComponentRefinement(WingData wingData, Wing wing, WingSaveData wingSaveData = null) 
    {
        Initialize(wingData, wing, wingSaveData);
    }

    public void Initialize(WingData wingData, Wing wing, WingSaveData wingSaveData = null) 
    {
        this.wing = wing;

        if (wingSaveData != null) 
        {
            CurrentRank = wingSaveData.CurrentRank;
            CurrentRankProgress = wingSaveData.CurrentRankProgress;
        } else 
        {
            CurrentRank = WingRefinementRank.Star0;
            CurrentRankProgress = 0;
        }
    }

    public int GetRefinementThreshold() => wing.WingEvolutionGrowthProfile.GetRefinementThreshold(this.CurrentRank);

    private WingRefinementRank TryGetNextRank(WingRefinementRank rank)
    {
        int nextValue = (int)rank + 1;

        if (!Enum.IsDefined(typeof(WingRefinementRank), nextValue))
            return rank; // stay at max rank

        return (WingRefinementRank)nextValue;
    }

    public bool AddDuplicate(Wing duplicate)
    {
        if (duplicate.WingId != wing.WingId) return false;

        CurrentRankProgress++;

        CheckRankUp();

        return true;
    }

    private void CheckRankUp()
    {
        while (true)
        {
            int threshold = GetRefinementThreshold();

            if (CurrentRankProgress < threshold) break;

            var nextRank = TryGetNextRank(CurrentRank);

            // Already max rank
            if (nextRank == CurrentRank)
            {
                CurrentRankProgress = threshold;
                break;
            }

            CurrentRankProgress -= threshold;
            CurrentRank = nextRank;
        }
    }

    public void ForceMaxRefinement()
    {
        // Safely determine the highest possible enum value defined in WingRefinementRank
        var enumValues = Enum.GetValues(typeof(WingRefinementRank));
        if (enumValues.Length > 0)
            CurrentRank = (WingRefinementRank)enumValues.GetValue(enumValues.Length - 1);

        CurrentRankProgress = 0;
    }

    public float GetElementMatchBonus() 
    {
        return 0.05f * (CurrentRankIndex +1);
    }

}
