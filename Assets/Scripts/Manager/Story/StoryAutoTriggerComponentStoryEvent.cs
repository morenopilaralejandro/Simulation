using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryAutoTriggerComponentStoryEvent
{
    public StoryEvent StoryEvent { get; private set; }

    public StoryAutoTriggerComponentStoryEvent(StoryAutoTriggerData storyAutoTriggerData, StoryAutoTrigger storyAutoTrigger)
    {
        StoryEvent = StoryEventFactory.CreateById(storyAutoTriggerData.StoryAutoTriggerId);
    }
}
