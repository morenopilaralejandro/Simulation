using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryEventComponentAttributes
{
    public string StoryEventId { get; private set; }

    public StoryEventComponentAttributes(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        StoryEventId = storyEventData.StoryEventId;
    }
}
