using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingManagerStorage
{
    private Dictionary<string, Wing> wings = new();

    public IReadOnlyDictionary<string, Wing> Wings => wings;
    public int Count => wings.Count;

    public WingManagerStorage() { }

    #region First Time Initialize

    public void FirstTimeInitialize()
    {
        WingDatabase wingDatabase = WingDatabase.Instance;

        AddWing(wingDatabase.GetWingData("wing-00001-angel"));
        AddWing(wingDatabase.GetWingData("wing-00002-devil"));
    }

    #endregion

    #region Add / Remove

    public Wing AddWing(Wing wing)
    {
        if (wings.ContainsKey(wing.WingGuid))
        {
            LogManager.Warning($"[WingManagerStorage] Wing with GUID {wing.WingGuid} already exists. Skipping.");
            return wings[wing.WingGuid];
        }

        wings[wing.WingGuid] = wing;
        WingEvents.RaiseWingAdded(wing);

        LogManager.Info($"[WingManagerStorage] Added wing: {wing.WingId} ({wing.WingGuid})");
        return wing;
    }

    public Wing AddWing(WingData wingData)
    {
        Wing wing = WingFactory.CreateFromData(wingData.WingId);
        return AddWing(wing);
    }

    public bool RemoveWing(string wingGuid)
    {
        if (wings.TryGetValue(wingGuid, out Wing wing))
        {
            wings.Remove(wingGuid);
            WingEvents.RaiseWingRemoved(wing);
            LogManager.Info($"[WingManagerStorage] Removed wing: {wing.WingId} ({wingGuid})");
            return true;
        }

        LogManager.Warning($"[WingManagerStorage] Wing with GUID {wingGuid} not found for removal.");
        return false;
    }

    #endregion

    #region Query

    public Wing GetWing(string wingGuid)
    {
        wings.TryGetValue(wingGuid, out Wing wing);
        return wing;
    }

    public bool HasWing(string wingGuid)
    {
        return wings.ContainsKey(wingGuid);
    }

    public List<Wing> GetAllWings()
    {
        return wings.Values.ToList();
    }

    public List<Wing> GetWingsByElement(Element element)
    {
        return wings.Values.Where(w => w.ContainsElement(element)).ToList();
    }

    public List<Wing> GetWingsByElementMatchingInheritance(Element[] elements)
    {
        List<Wing> result = new();
        bool canAdd = false;

        foreach(Wing wing in wings.Values) 
        {
            canAdd = false;
            for(int i = 0; i < elements.Length; i++)
            {
                if (wing.ContainsElement(elements[i]))
                {
                    canAdd = true;
                }
            }

            if (canAdd) 
                result.Add(wing);
        }
            
        return result;
    }

    #endregion

    #region Persistence
    
    public WingStorageSaveData Export()
    {
        WingStorageSaveData saveData = new WingStorageSaveData();
        saveData.WingSaveDataList = new List<WingSaveData>();

        foreach (Wing wing in wings.Values)
            saveData.WingSaveDataList.Add(wing.Export());

        return saveData;
    }

    public void Import(WingStorageSaveData saveData)
    {
        wings.Clear();

        if (saveData?.WingSaveDataList == null) return;

        foreach (WingSaveData wingSaveData in saveData.WingSaveDataList)
        {
            AddWing(WingFactory.CreateFromSaveData(wingSaveData));
        }
    }

    public void Clear()
    {
        wings.Clear();
    }

    #endregion

}
