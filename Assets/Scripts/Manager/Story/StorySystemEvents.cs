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
        storyEventDatabase = StoryEventDatabase.Instance;
    }

    public void TriggerStoryEvent(string storyEventId)
    {
        if (completedEventsHashSet.Contains(storyEventId))
        {
            LogManager.Warning($"[StorySystemEvents] Event '{storyEventId}' already triggered.");
            return;
        }

        completedEventsHashSet.Add(storyEventId);
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
        return completedEventsHashSet.Contains(storyEventId);
    }

    private void ProcessStoryEvent(StoryEvent storyEvent)
    {
        foreach (var effect in storyEvent.StoryEffects)
        {
            switch (effect.EffectType)
            {
                case StoryEffectType.SetFlag:
                    storySystemManager.SetFlag(effect.TargetId, effect.BoolValue);
                    break;
                case StoryEffectType.SetVariable:
                    storySystemManager.SetVariable(effect.TargetId, effect.IntValue);
                    break;
                case StoryEffectType.IncrementVariable:
                    storySystemManager.IncrementVariable(effect.TargetId, effect.IntValue);
                    break;
                case StoryEffectType.StartQuest:
                    questSystemManager.StartQuest(effect.TargetId);
                    break;
                case StoryEffectType.CompleteQuest:
                    questSystemManager.CompleteQuest(effect.TargetId);
                    break;
                case StoryEffectType.AdvanceChapter:
                    storySystemManager.AdvanceChapter();
                    break;
            }
        }
    }

    public void Import(StorySystemSaveData saveData) 
    {
        completedEventsHashSet = new HashSet<string>(saveData.CompletedEventsList);
    }
}
