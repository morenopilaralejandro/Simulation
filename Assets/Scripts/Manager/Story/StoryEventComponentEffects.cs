using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryEventComponentEffects
{
    private List<StoryEffect> storyEffects = new List<StoryEffect>();
    public IReadOnlyList<StoryEffect> StoryEffects => storyEffects;

    public StoryEventComponentEffects(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        storyEffects = storyEventData.StoryEffects;
    }
}
