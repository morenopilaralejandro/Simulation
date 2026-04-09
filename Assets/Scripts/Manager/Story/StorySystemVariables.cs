using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StorySystemVariables
{
    private StorySystemManager storySystemManager;

    private Dictionary<string, int> variablesDict = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> VariablesDict => variablesDict;

    public StorySystemVariables() 
    {
        storySystemManager = StorySystemManager.Instance;
    }

    public void SetVariable(string variableId, int intValue)
    {
        variablesDict[variableId] = intValue;
        StoryEvents.RaiseStoryVariableChanged(variableId, intValue);
        
        LogManager.Trace($"[StorySystemVariables] Variable '{variableId}' set to {intValue}");
        storySystemManager.EvaluateTriggers();
    }

    public void IncrementVariable(string variableId, int amount = 1)
    {
        int current = GetVariable(variableId);
        SetVariable(variableId, current + amount);
    }

    public int GetVariable(string variableId)
    {
        return variablesDict.TryGetValue(variableId, out int intValue) ? intValue : 0;
    }

    public void Import(StorySystemSaveData saveData) 
    {
        foreach (var serializableKeyValue in saveData.VariablesList) 
            variablesDict[serializableKeyValue.Key] = serializableKeyValue.Value;
    }
}
