using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StorySystemTriggers
{
    private StorySystemManager storySystemManager;
    private QuestSystemManager questSystemManager;

    private HashSet<string> triggeredHashSet = new HashSet<string>();
    private Dictionary<string, StoryAutoTrigger> storyAutoTriggerDictionary;

    public IReadOnlyCollection<string> TriggeredCollection => triggeredHashSet;

    public StorySystemTriggers() 
    {
        storySystemManager = StorySystemManager.Instance;
        questSystemManager = QuestSystemManager.Instance;
        InitializeStoryAutoTriggers();
    }

    private void InitializeStoryAutoTriggers()
    {
        storyAutoTriggerDictionary = new Dictionary<string, StoryAutoTrigger>();
        
        foreach (var storyAutoTriggerData in DatabaseManager.Instance.DatabaseRegistry.StoryAutoTriggerData.Data.Values)
        {
            storyAutoTriggerDictionary[storyAutoTriggerData.StoryAutoTriggerId] = new StoryAutoTrigger(storyAutoTriggerData);
        }
    }

    public void TriggerAutoTrigger(string storyAutoTriggerId)
    {
        triggeredHashSet.Add(storyAutoTriggerId);
    }

    public bool HasAutoTriggerTriggered(string storyAutoTriggerId)
    {
        return triggeredHashSet.Contains(storyAutoTriggerId);
    }

    public void EvaluateTriggers()
    {
        foreach (var storyAutoTrigger in storyAutoTriggerDictionary.Values)
        {
            if (storyAutoTrigger.HasTriggered) continue;
            if (!questSystemManager.CheckPrerequisites(storyAutoTrigger.Prerequisites)) continue;

            TriggerAutoTrigger(storyAutoTrigger.StoryAutoTriggerId);
            storySystemManager.TriggerStoryEvent(storyAutoTrigger.StoryEvent.StoryEventId);
        }
    }

    public StoryAutoTrigger GetStoryAutoTrigger(string storyAutoTriggerId)
    {
        return storyAutoTriggerDictionary.TryGetValue(storyAutoTriggerId, out var trigger) ? trigger : null;
    }

    public void Import(StorySystemSaveData saveData) 
    {
        triggeredHashSet = new HashSet<string>(saveData.TriggeredList);
    }
}
