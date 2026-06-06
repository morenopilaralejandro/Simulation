using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingComponentAttributes
{
    //private Wing wing;

    public string WingId { get; private set; }
    public string WingGuid { get; private set; }

    public WingComponentAttributes(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        Initialize(wingData, wing, wingSaveData);
    }

    public void Initialize(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        WingId = wingData.WingId;

        if (wingSaveData == null) 
        {
            WingGuid = Guid.NewGuid().ToString();
        } else 
        {
            WingGuid = wingSaveData.WingGuid;
        }
    }
}
