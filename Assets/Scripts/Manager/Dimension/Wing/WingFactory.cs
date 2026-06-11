using UnityEngine;
using System;
using System.Collections.Generic;

public static class WingFactory
{
    public static Wing CreateFromSaveData(WingSaveData saveData) 
    {
        WingData data = WingDatabase.Instance.GetWingData(saveData.WingId);
        Wing obj = new Wing(data, saveData);
        return obj;
    }

    public static Wing CreateFromData(string id) 
    {
        WingData data = WingDatabase.Instance.GetWingData(id);
        Wing obj = new Wing(data);
        return obj;
    }
}
