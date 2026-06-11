using UnityEngine;
using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingComponentAppearance
{
    #region Fields

    public WingType WingType { get; private set; }
    public WingColorType WingColorTypeDefault { get; private set; }
    public WingColorType WingColorTypeDye { get; private set; }
    public WingColorType WingColorType => HasDye ? WingColorTypeDye : WingColorTypeDefault;
    public bool HasDye { get; private set; }

    #endregion

    #region Initialization

    public WingComponentAppearance(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        Initialize(wingData, wing, wingSaveData);
    }

    public void Initialize(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        WingType = wingData.WingType;
        WingColorTypeDefault = wingData.WingColorType;

        if (wingSaveData != null) 
        {
            HasDye = wingSaveData.HasDye;
            WingColorTypeDye = wingSaveData.WingColorTypeDye; 
        } else 
        {
            HasDye = false;
        }
    }

    #endregion

    #region Helpers

    public void SetHasDye(bool boolValue) 
    {
        HasDye = boolValue;
    }

    public void SetWingColorTypeDye(WingColorType wingColorType)
    {
        WingColorTypeDye = wingColorType;
    }

    #endregion
}
