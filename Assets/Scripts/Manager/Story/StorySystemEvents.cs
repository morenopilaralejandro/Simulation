using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StorySystemEvents
{
    private StorySystemManager storySystemManager;
    private QuestSystemManager questSystemManager;
    private StoryEventDatabase storyEventDatabase;
    private StoryEvent auxStoryEvent;

    private HashSet<string> completedEventsHashSet = new HashSet<string>();
    public IReadOnlyCollection<string> CompletedEventsCollection => completedEventsHashSet;

    public StorySystemEvents() 
    {
        storySystemManager = StorySystemManager.Instance;
        questSystemManager = QuestSystemManager.Instance;
        storyEventDatabase = storyEventDatabase.Instance;
    }

    public void TriggerStoryEvent(string storyEventId)
    {
        if (completedEventsHashSet.Contains(storyEventId))
        {
            LogManager.Warning($"[StorySystemEvents] Event '{storyEventId}' already triggered.");
            return;
        }

        completedEventsList.Add(storyEventId);
        StoryEvents.RaiseStoryEventTriggered(storyEventId);
        LogManager.Trace($"[StorySystemEvents] Event '{storyEventId}' triggered.");

        StoryEventData storyEventData = storyEventDatabase.GetStoryEventData(storyEventId);
        if (storyEventData != null) 
        {
            ProcessStoryEvent(new StoryEvent(storyEventData));
        } else 
        {
            LogManager.Error($"[StorySystemEvents] No data for '{storyEventId}'.");
        }
    }

    public bool IsEventCompleted(string storyEventId)
    {
        return completedEventsList.Contains(storyEventId);
    }

    private void ProcessStoryEvent(StoryEvent storyEvent)
    {
        foreach (var effect in storyEvent.Effects)
        {
            switch (effect.EffectType)
            {
                case StoryEffectType.SetFlag:
                    storySystemManager.SetFlag(effect.targetName, effect.boolValue);
                    break;
                case StoryEffectType.SetVariable:
                    storySystemManager.SetVariable(effect.targetName, effect.intValue);
                    break;
                case StoryEffectType.IncrementVariable:
                    storySystemManager.IncrementVariable(effect.targetName, effect.intValue);
                    break;
                case StoryEffectType.StartQuest:
                    questSystemManager.StartQuest(effect.targetName);
                    break;
                case StoryEffectType.CompleteQuest:
                    questSystemManager.CompleteQuest(effect.targetName);
                    break;
                case StoryEffectType.AdvanceChapter:
                    storySystemManager.AdvanceChapter();
                    break;
            }
        }
    }
}
