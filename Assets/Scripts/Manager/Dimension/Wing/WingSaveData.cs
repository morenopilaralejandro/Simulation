using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

[System.Serializable]
public class WingSaveData
{
    public string WingId;
    public string WingGuid;

    public bool HasDye;
    public WingColorType WingColorTypeDye;

    public List<WingStatSaveData> IndividualStats;

    public WingEvolution CurrentEvolution;
    public int TimesUsedTotal;
    public int TimesUsedCurrentEvolution;

    public WingRefinementRank CurrentRank;
    public int CurrentRankProgress;
}
