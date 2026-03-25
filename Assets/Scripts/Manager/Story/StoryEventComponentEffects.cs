using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryEventComponentEffects
{
    private List<StoryEffect> effects = new List<StoryEffect>();
    public IReadOnlyList<StoryEffect> Effects => prerequisites;

    public StoryEventComponentPrerequisites(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        effects = storyEventData.Effects;
    }
}
