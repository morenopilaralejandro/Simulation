using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Scout;

public class ScoutTierComponentAttributes
{
    public string ScoutTierId { get; private set; }
    public string UnlockFlag { get; private set; }

    public ScoutTierComponentAttributes(ScoutTierData data)
    {
        ScoutTierId = data.ScoutTierId;
        UnlockFlag = data.UnlockFlag;
    }
}
