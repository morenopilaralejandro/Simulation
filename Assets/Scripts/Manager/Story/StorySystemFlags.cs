using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StorySystemFlags
{
    private StorySystemManager storySystemManager;

    private Dictionary<string, bool> flagsDict = new Dictionary<string, bool>();
    public IReadOnlyDictionary<string, bool> FlagsDict => flagsDict;

    public StorySystemFlags() 
    {
        storySystemManager = StorySystemManager.Instance;
    }

    public void SetFlag(string flagId, bool boolValue)
    {
        flagsDict[flagId] = boolValue;
        StoryEvents.RaiseStoryFlagChanged(flagId);
        LogManager.Trace($"[StorySystemFlags] Flag '{flagId}' set to {boolValue}");
        storySystemManager.EvaluateTriggers();
    }

    public bool GetFlag(string flagId)
    {
        return flagsDict.TryGetValue(flagId, out bool boolValue) && boolValue;
    }

    public bool HasFlag(string flagId)
    {
        return flagsDict.ContainsKey(flagId);
    }

    public void Import(StorySystemSaveData saveData) 
    {
        foreach (var serializableKeyValue in saveData.FlagsList) 
            flagsDict[serializableKeyValue.Key] = serializableKeyValue.Value;
    }
}
