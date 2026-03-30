using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StorySystemTriggers
{
    private StorySystemManager storySystemManager;
    private QuestSystemManager questSystemManager;
    private StoryAutoTriggerDatabase storyAutoTriggerDatabase;

    private HashSet<string> triggeredHashSet = new HashSet<string>();
    public IReadOnlyCollection<string> TriggeredCollection => triggeredHashSet;

    public StorySystemTriggers() 
    {
        storySystemManager = StorySystemManager.Instance;
        questSystemManager = QuestSystemManager.Instance;
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
        foreach (var storyAutoTrigger in storyAutoTriggerDatabase.StoryAutoTriggerDict.Values)
        {
            if (storyAutoTrigger.HasTriggered) continue;
            if (!questSystemManager.CheckPrerequisites(storyAutoTrigger.Prerequisites)) continue;

            TriggerAutoTrigger(storyAutoTrigger.StoryAutoTriggerId);
            storySystemManager.TriggerStoryEvent(storyAutoTrigger.StoryEvent.StoryEventId);
        }
    }

    public void Import(StorySystemSaveData saveData) 
    {
        triggeredHashSet = new HashSet<string>(saveData.TriggeredList);
    }
}
