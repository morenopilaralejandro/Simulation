using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryAutoTriggerComponentAttributes
{
    public string StoryAutoTriggerId { get; private set; }

    public StoryAutoTriggerComponentAttributes(StoryAutoTriggerData storyAutoTriggerData, StoryAutoTrigger storyAutoTrigger)
    {
        StoryAutoTriggerId = storyAutoTriggerData.StoryAutoTriggerId;
    }
}
