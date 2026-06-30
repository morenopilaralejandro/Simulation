using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingComponentPersistence
{
    #region Fields

    private Wing wing;

    #endregion        

    #region LifeCycle

    public WingComponentPersistence(WingData wingData, Wing wing)
    {
        Initialize(wingData, wing);
    }

    public void Initialize(WingData wingData, Wing wing)
    {
        this.wing = wing;
    }

    #endregion

    #region Import

    public void Import(WingSaveData saveData)
    {
        WingData data = DatabaseManager.Instance.GetWingData(saveData.WingId);
        wing.Initialize(data, saveData);
    }

    #endregion

    #region Export

    public WingSaveData Export()
    {
        return new WingSaveData
        {
            // atributtes
            WingId = wing.WingId,
            WingGuid = wing.WingGuid,

            IndividualStats = GetIndividualStatsForSave()
        };
    }

    #endregion

    #region Helpers

    private List<WingStatSaveData> GetIndividualStatsForSave()
    {
        List<WingStatSaveData> individualStats = new List<WingStatSaveData>();
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            if (stat == Stat.Hp || stat == Stat.Sp) continue;
            individualStats.Add(new WingStatSaveData
            {
                Stat = stat,
                Val = wing.GetIndividualStat(stat)
            });
        }
        return individualStats;
    }

    #endregion

}
