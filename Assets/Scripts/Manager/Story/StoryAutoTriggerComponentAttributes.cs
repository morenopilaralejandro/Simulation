using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryAutoTriggerComponentAttributes
{
    public string StoryAutoTriggerId { get; private set; }

    public StoryAutoTriggerComponentAttributes(StoryAutoTriggerData storyAutoTriggerData, StoryAutoTrigger storyAutoTrigger)
    {
        StoryAutoTriggerId = storyAutoTriggerData.StoryAutoTriggerId;
    }
}
