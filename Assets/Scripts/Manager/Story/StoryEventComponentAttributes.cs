using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryEventComponentAttributes
{
    public string StoryEventId { get; private set; }

    public StoryEventComponentAttributes(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        StoryEventId = storyEventData.StoryEventId;
    }
}
